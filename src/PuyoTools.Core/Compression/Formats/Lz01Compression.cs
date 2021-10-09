using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Compression
{
    public class Lz01Compression : CompressionBase
    {
        private static readonly byte[] magicCode = { (byte)'L', (byte)'Z', (byte)'0', (byte)'1' };

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Decompress(Stream source, Stream destination)
        {
            source.Position += 4;

            // Get the source length and the destination length
            int sourceLength      = PTStream.ReadInt32(source);
            int destinationLength = PTStream.ReadInt32(source);

            source.Position += 4;

            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x10;
            int destinationPointer = 0x0;
            int bufferPointer      = 0xFEE;

            // Initalize the buffer
            byte[] buffer = new byte[0x1000];

            // Start decompression
            while (sourcePointer < sourceLength)
            {
                byte flag = PTStream.ReadByte(source);
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x1) != 0) // Not compressed
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

                        int matchOffset = (((b2 >> 4) & 0xF) << 8) | b1;
                        int matchLength = (b2 & 0xF) + 3;

                        for (int j = 0; j < matchLength; j++)
                        {
                            destination.WriteByte(buffer[(matchOffset + j) & 0xFFF]);
                            destinationPointer++;

                            buffer[bufferPointer] = buffer[(matchOffset + j) & 0xFFF];
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

                    flag >>= 1;
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
            var destinationStartPosition = destination.Position;

            // Get the source length, and read it in to an array
            int sourceLength = (int)(source.Length - source.Position);
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
            LzBufferDictionary dictionary = new LzBufferDictionary();
            dictionary.SetBufferSize(0x1000);
            dictionary.SetBufferStart(0xFEE);
            dictionary.SetMaxMatchAmount(0xF + 3);

            using (var writer = destination.AsBinaryWriter())
            using (var buffer = new MemoryStream(16)) // Will never contain more than 16 bytes
            using (var bufferWriter = new BinaryWriter(buffer))
            {
                // Write out the header
                // Magic code
                writer.Write(magicCode);
                writer.WriteInt32(0); // Compressed length (will be filled in later)
                writer.WriteInt32(sourceLength); // Decompressed length
                writer.WriteInt32(0);

                // Start compression
                while (sourcePointer < sourceLength)
                {
                    byte flag = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        // Search for a match
                        int[] match = dictionary.Search(sourceArray, (uint)sourcePointer, (uint)sourceLength);

                        if (match[1] > 0) // There is a match
                        {
                            bufferWriter.WriteUInt16((ushort)((match[0] & 0xFF) | (match[0] & 0xF00) << 4 | ((match[1] - 3) & 0xF) << 8));

                            dictionary.AddEntryRange(sourceArray, sourcePointer, match[1]);

                            sourcePointer += match[1];
                        }
                        else // There is not a match
                        {
                            flag |= (byte)(1 << i);

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

                // Go back to the beginning of the file and write out the compressed length
                var destinationLength = (int)(destination.Position - destinationStartPosition);
                writer.At(destinationStartPosition + 4, x => x.WriteInt32(destinationLength));
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
                return remainingLength > 16
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)
                    && reader.At(startPosition + 4, x => x.ReadInt32()) == remainingLength;
            }
        }
    }
}