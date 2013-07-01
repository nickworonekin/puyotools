using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class CnxCompression : CompressionBase
    {
        /*
         * CNX decompression support by drx (Luke Zapart)
         * <thedrx@gmail.com>
         */

        public override string Name
        {
            get { return "CNX"; }
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
        public override void Decompress(Stream source, Stream destination, int length)
        {
            byte[] sourceBuffer = new byte[length];
            source.Read(sourceBuffer, 0, length);

            // Set up information for decompression
            int sourcePointer = 0x10;
            int destPointer   = 0x0;

            int sourceLength = PTMethods.ToInt32BE(sourceBuffer, 0x8) + 16;
            int destLength   = PTMethods.ToInt32BE(sourceBuffer, 0xC);

            byte[] destBuffer = new byte[destLength];

            // Start Decompression
            while (sourcePointer < length && destPointer < destLength)
            {
                byte flag = sourceBuffer[sourcePointer]; // Compression Flag
                sourcePointer++;

                for (int i = 0; i < 4; i++)
                {
                    int distance, amount, value;

                    switch (flag & 0x3)
                    {
                        // Padding mode
                        // All CNX files seem to be packed in 0x800 chunks. when nearing
                        // a 0x800 cutoff, there usually is a padding command at the end to skip
                        // a few bytes (to the next 0x800 chunk, i.e. 0x4800, 0x7000, etc.)
                        case 0:
                            sourcePointer += (sourceBuffer[sourcePointer] + 1);
                            i = 3;
                            break;

                        // Data is not compressed
                        // Single byte copy mode
                        case 1:
                            destBuffer[destPointer] = sourceBuffer[sourcePointer];
                            sourcePointer++;
                            destPointer++;
                            break;

                        // Data is compressed
                        case 2:
                            value = PTMethods.ToUInt16BE(sourceBuffer, sourcePointer);
                            sourcePointer += 2;

                            distance = (value >> 5) + 1;
                            amount = (value & 0x1F) + 4;

                            // Copy the data
                            for (int j = 0; j < amount; j++)
                            {
                                destBuffer[destPointer] = destBuffer[destPointer - distance];
                                destPointer++;
                            }

                            break;

                        // Data is not compressed
                        // Block copy mode
                        case 3:
                            amount = sourceBuffer[sourcePointer];
                            sourcePointer++;

                            // Copy the data
                            for (int j = 0; j < amount; j++)
                            {
                                destBuffer[destPointer] = sourceBuffer[sourcePointer];
                                sourcePointer++;
                                destPointer++;
                            }

                            break;
                    }

                    // Check for out of range
                    if (sourcePointer >= length || destPointer >= destLength)
                        break;

                    flag >>= 2;
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
        public override void Compress(Stream source, Stream destination, int length)
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
            return (length > 16 &&
                PTStream.Contains(source, 0, new byte[] { (byte)'C', (byte)'N', (byte)'X', 0x2 }) &&
                PTStream.ReadInt32BEAt(source, source.Position + 8) + 16 == length);
        }
    }
}