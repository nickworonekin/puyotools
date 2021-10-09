using System;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Compression
{
    public class Lz10Compression : CompressionBase
    {
        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /*public override void Decompress(Stream source, Stream destination)
        {
            // Get the source and destination length
            int sourceLength      = (int)(source.Length - source.Position);
            int destinationLength = PTStream.ReadInt32(source) >> 8;

            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x4;
            int destinationPointer = 0x0;
            int bufferPointer      = 0x0;

            // Initalize the buffer
            byte[] buffer = new byte[0x1000];

            // Start decompression
            while (sourcePointer < sourceLength)
            {
                byte flag = PTStream.ReadByte(source);
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x80) == 0) // Not compressed
                    {
                        byte value = PTStream.ReadByte(source);
                        sourcePointer++;

                        destination.WriteByte(value);
                        destinationPointer++;

                        buffer[bufferPointer] = value;
                        bufferPointer = (bufferPointer + 1) & 0xFFF;
                    }
                    else // Compressed
                    {
                        byte b1 = PTStream.ReadByte(source), b2 = PTStream.ReadByte(source);
                        sourcePointer += 2;

                        int matchDistance = (((b1 & 0xF) << 8) | b2) + 1;
                        int matchLength   = (b1 >> 4) + 3;

                        for (int j = 0; j < matchLength; j++)
                        {
                            destination.WriteByte(buffer[(bufferPointer - matchDistance) & 0xFFF]);
                            destinationPointer++;

                            buffer[bufferPointer] = buffer[(bufferPointer - matchDistance) & 0xFFF];
                            bufferPointer = (bufferPointer + 1) & 0xFFF;
                        }
                    }

                    // Check to see if we reached the end of the source
                    if (sourcePointer >= sourceLength)
                    {
                        break;
                    }

                    // Check to see if we wrote too much data to the destination
                    if (destinationPointer > destinationLength)
                    {
                        throw new Exception("Too much data written to the destination.");
                    }

                    flag <<= 1;
                }
            }
        }*/

        public override void Decompress(Stream source, Stream destination)
        {
            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                // Get the source and destination length
                int sourceLength = (int)(source.Length - source.Position);
                int destinationLength = reader.ReadInt32() >> 8;

                // Set the source, destination, and buffer pointers
                int sourcePointer = 0x4;
                int destinationPointer = 0x0;
                int bufferPointer = 0x0;

                // Initalize the buffer
                byte[] buffer = new byte[0x1000];

                // Start decompression
                while (sourcePointer < sourceLength)
                {
                    byte flag = reader.ReadByte();
                    sourcePointer++;

                    for (int i = 0; i < 8; i++)
                    {
                        if ((flag & 0x80) == 0) // Not compressed
                        {
                            byte value = reader.ReadByte();
                            sourcePointer++;

                            destination.WriteByte(value);
                            destinationPointer++;

                            buffer[bufferPointer] = value;
                            bufferPointer = (bufferPointer + 1) & 0xFFF;
                        }
                        else // Compressed
                        {
                            var matchPair = reader.ReadUInt16BigEndian();
                            sourcePointer += 2;

                            int matchDistance = (matchPair & 0xFFF) + 1;
                            int matchLength = ((matchPair >> 12) & 0xF) + 3;

                            for (int j = 0; j < matchLength; j++)
                            {
                                destination.WriteByte(buffer[(bufferPointer - matchDistance) & 0xFFF]);
                                destinationPointer++;

                                buffer[bufferPointer] = buffer[(bufferPointer - matchDistance) & 0xFFF];
                                bufferPointer = (bufferPointer + 1) & 0xFFF;
                            }
                        }

                        // Check to see if we reached the end of the source
                        if (sourcePointer >= sourceLength)
                        {
                            break;
                        }

                        // Check to see if we wrote too much data to the destination
                        if (destinationPointer > destinationLength)
                        {
                            throw new Exception("Too much data written to the destination.");
                        }

                        flag <<= 1;
                    }
                }
            }
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Compress(Stream source, Stream destination)
        {
            // Get the source length
            int sourceLength = (int)(source.Length - source.Position);

            // LZ10 compression can only handle files smaller than 16MB
            if (sourceLength > 0xFFFFFF)
            {
                throw new Exception($"LZ10 compression can't be used to compress files larger than {0xFFFFFF:N0} bytes.");
            }

            // Read the source data into an array
            byte[] sourceArray = new byte[sourceLength];
            var totalSourceBytesRead = 0;
            int sourceBytesRead;
            do
            {
                sourceBytesRead = source.Read(sourceArray, totalSourceBytesRead, sourceLength - totalSourceBytesRead);
                if (sourceBytesRead == 0)
                {
                    throw new IOException($"Unable to read all bytes in {nameof(source)}");
                }
                totalSourceBytesRead += sourceBytesRead;
            }
            while (totalSourceBytesRead < sourceLength);

            // Set the source and destination pointers
            int sourcePointer = 0x0;
            //int destinationPointer = 0x4;

            // Initalize the LZ dictionary
            LzWindowDictionary dictionary = new LzWindowDictionary();
            dictionary.SetWindowSize(0x1000);
            dictionary.SetMaxMatchAmount(0xF + 3);

            using (var writer = destination.AsBinaryWriter())
            using (var buffer = new MemoryStream(16)) // Will never contain more than 16 bytes
            using (var bufferWriter = new BinaryWriter(buffer))
            {
                // Write out the header
                // Magic code & decompressed length
                writer.WriteInt32(0x10 | (sourceLength << 8));

                // Start compression
                while (sourcePointer < sourceLength)
                {
                    byte flag = 0;

                    for (int i = 7; i >= 0; i--)
                    {
                        // Search for a match
                        int[] match = dictionary.Search(sourceArray, (uint)sourcePointer, (uint)sourceLength);

                        if (match[1] > 0) // There is a match
                        {
                            flag |= (byte)(1 << i);

                            bufferWriter.WriteUInt16BigEndian((ushort)((match[1] - 3) << 12 | ((match[0] - 1) & 0xFFF)));

                            dictionary.AddEntryRange(sourceArray, sourcePointer, match[1]);

                            sourcePointer += match[1];
                        }
                        else // There is not a match
                        {
                            bufferWriter.WriteByte(sourceArray[sourcePointer]);

                            dictionary.AddEntry(sourceArray, sourcePointer);

                            sourcePointer++;
                        }

                        // Check to see if we reached the end of the file
                        if (sourcePointer >= sourceLength)
                            break;
                    }

                    // Flush the buffer and write it to the destination stream
                    writer.WriteByte(flag);

                    buffer.WriteTo(destination);
                    buffer.SetLength(0);
                }
            }
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;

            using (var reader = source.AsBinaryReader())
            {
                return source.Length - startPosition > 5
                    && reader.At(startPosition, x => x.ReadByte()) == 0x10
                    && reader.At(startPosition + 4, x => x.ReadByte()) >> 5 == 0;
            }
        }
    }
}