using System;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Compression
{
    public class Lz11Compression : CompressionBase
    {
        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /*public override void Decompress(Stream source, Stream destination)
        {
            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x4;
            int destinationPointer = 0x0;
            int bufferPointer      = 0x0;

            // Get the source and destination length
            int sourceLength      = (int)(source.Length - source.Position);
            int destinationLength = PTStream.ReadInt32(source) >> 8;
            if (destinationLength == 0)
            {
                // If the destination length is larger than 0xFFFFFF, then the next 4 bytes is the destination length
                destinationLength = PTStream.ReadInt32(source);
                sourcePointer += 4;
            }

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
                    else // Data is compressed
                    {
                        int matchDistance, matchLength;

                        // Read in the first 2 bytes (since they are used for all of these)
                        byte b1 = PTStream.ReadByte(source), b2 = PTStream.ReadByte(source), b3, b4;
                        sourcePointer += 2;

                        // Let's determine how many bytes the distance & length pair take up
                        switch (b1 >> 4)
                        {
                            case 0: // 3 bytes
                                b3 = PTStream.ReadByte(source);
                                sourcePointer++;

                                matchDistance = (((b2 & 0xF) << 8) | b3) + 1;
                                matchLength = (((b1 & 0xF) << 4) | (b2 >> 4)) + 17;
                                break;

                            case 1: // 4 bytes
                                b3 = PTStream.ReadByte(source);
                                b4 = PTStream.ReadByte(source);
                                sourcePointer += 2;

                                matchDistance = (((b3 & 0xF) << 8) | b4) + 1;
                                matchLength = (((b1 & 0xF) << 12) | (b2 << 4) | (b3 >> 4)) + 273;
                                break;

                            default: // 2 bytes
                                matchDistance = (((b1 & 0xF) << 8) | b2) + 1;
                                matchLength = (b1 >> 4) + 1;
                                break;
                        }

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
            // Set the source, destination, and buffer pointers
            int sourcePointer = 0x4;
            int destinationPointer = 0x0;
            int bufferPointer = 0x0;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                // Get the source and destination length
                int sourceLength = (int)(source.Length - source.Position);
                int destinationLength = reader.ReadInt32() >> 8;
                if (destinationLength == 0)
                {
                    // If the destination length is larger than 0xFFFFFF, then the next 4 bytes is the destination length
                    destinationLength = reader.ReadInt32();
                    sourcePointer += 4;
                }

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
                        else // Data is compressed
                        {
                            int matchDistance, matchLength;

                            // Read in the first 2 bytes (since they are used for all of these)
                            int matchPair = reader.ReadUInt16BigEndian();
                            sourcePointer += 2;

                            // Let's determine how many bytes the distance & length pair take up
                            switch ((matchPair >> 12) & 0xF)
                            {
                                case 0: // 3 bytes
                                    matchPair = matchPair << 8 | reader.ReadByte();
                                    sourcePointer++;

                                    matchDistance = (matchPair & 0xFFF) + 1;
                                    matchLength = ((matchPair >> 12) & 0xFF) + 17;
                                    break;

                                case 1: // 4 bytes
                                    matchPair = matchPair << 16 | reader.ReadUInt16BigEndian();
                                    sourcePointer += 2;

                                    matchDistance = (matchPair & 0xFFF) + 1;
                                    matchLength = ((matchPair >> 12) & 0xFFFF) + 273;
                                    break;

                                default: // 2 bytes
                                    matchDistance = (matchPair & 0xFFF) + 1;
                                    matchLength = ((matchPair >> 12) & 0xF) + 1;
                                    break;
                            }

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

            // Initalize the LZ dictionary
            LzWindowDictionary dictionary = new LzWindowDictionary();
            dictionary.SetWindowSize(0x1000);
            dictionary.SetMaxMatchAmount(0x1000);

            using (var writer = destination.AsBinaryWriter())
            using (var buffer = new MemoryStream(32)) // Will never contain more than 32 bytes
            using (var bufferWriter = new BinaryWriter(buffer))
            {
                // Write out the header
                // Magic code & decompressed length
                if (sourceLength <= 0xFFFFFF)
                {
                    writer.WriteInt32(0x11 | (sourceLength << 8));
                }
                else
                {
                    writer.WriteInt32(0x11);
                    writer.WriteInt32(sourceLength);
                }

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

                            // How many bytes will the match take up?
                            if (match[1] <= 0xF + 1) // 2 bytes
                            {
                                bufferWriter.WriteUInt16BigEndian((ushort)((match[1] - 1) << 12 | ((match[0] - 1) & 0xFFF)));
                            }
                            else if (match[1] <= 0xFF + 17) // 3 bytes
                            {
                                bufferWriter.WriteByte((byte)(((match[1] - 17) & 0xFF) >> 4));
                                bufferWriter.WriteUInt16BigEndian((ushort)((match[1] - 17) << 12 | ((match[0] - 1) & 0xFFF)));
                            }
                            else // 4 bytes
                            {
                                bufferWriter.WriteUInt32BigEndian((uint)(0x10000000 | ((match[1] - 273) & 0xFFFF) << 12 | ((match[0] - 1) & 0xFFF)));
                            }

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
            var remainingLength = source.Length - startPosition;

            using (var reader = source.AsBinaryReader())
            {
                if (!(remainingLength > 5
                    && reader.At(startPosition, x => x.ReadByte()) == 0x11))
                {
                    return false;
                }

                var decompressedLength = reader.At(startPosition, x => x.ReadInt32()) >> 8;

                if (decompressedLength != 0)
                {
                    return reader.At(startPosition + 4, x => x.ReadByte()) >> 5 == 0;
                }

                if (remainingLength > 9
                    && reader.At(startPosition + 4, x => x.ReadInt32()) != 0
                    && reader.At(startPosition + 8, x => x.ReadByte()) >> 5 == 0)
                {
                    return true;
                }

                return false;
            }
        }
    }
}