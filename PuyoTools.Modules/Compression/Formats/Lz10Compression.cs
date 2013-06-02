using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class Lz10Compression : CompressionBase
    {
        public override string Name
        {
            get { return "LZ10"; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public override void Decompress(Stream source2, Stream destination, int length)
        {
            byte[] source = new byte[length];
            source2.Read(source, 0, length);
            long offset = 0;

            // Set up information for decompression
            int sourcePointer = 0x4;
            int destPointer = 0x0;
            int destLength = (int)(BitConverter.ToUInt32(source, (int)offset) >> 8);
            byte[] destBuffer = new byte[destLength];

            // Start Decompression
            while (sourcePointer < length && destPointer < destLength)
            {
                byte flag = source[offset + sourcePointer]; // Compression Flag
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x80) == 0) // Data is not compressed
                    {
                        destBuffer[destPointer] = source[offset + sourcePointer];
                        sourcePointer++;
                        destPointer++;
                    }
                    else // Data is compressed
                    {
                        int distance = (((source[offset + sourcePointer] & 0xF) << 8) | source[offset + sourcePointer + 1]) + 1;
                        int amount = (source[offset + sourcePointer] >> 4) + 3;
                        sourcePointer += 2;

                        // Copy the data
                        for (int j = 0; j < amount; j++)
                        {
                            destBuffer[destPointer] = destBuffer[destPointer - distance];
                            destPointer++;
                        }
                    }

                    // Check for out of range
                    if (sourcePointer >= length || destPointer >= destLength)
                        break;

                    flag <<= 1;
                }
            }

            destination.Write(destBuffer, 0, destLength);
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public override void Compress(Stream source, Stream destination, int length, ModuleWriterSettings settings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines if the data is in the specified format.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="fname">Name of the file.</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 4 && PTStream.Contains(source, 0, new byte[] { 0x10 }));
        }
    }
}