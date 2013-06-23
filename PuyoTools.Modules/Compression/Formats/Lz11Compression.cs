using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class Lz11Compression : CompressionBase
    {
        public override string Name
        {
            get { return "LZ11"; }
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
            int sourcePointer = 0x4;
            int destPointer = 0x0;
            int destLength = BitConverter.ToInt32(sourceBuffer, 0) >> 8;

            // If the destination length is 0, then the length is stored in the next 4 bytes
            if (destLength == 0)
            {
                destLength = BitConverter.ToInt32(sourceBuffer, 4);
                sourcePointer += 4;
            }

            byte[] destBuffer = new byte[destLength];

            // Start Decompression
            while (sourcePointer < length && destPointer < destLength)
            {
                byte flag = sourceBuffer[sourcePointer]; // Compression Flag
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x80) == 0) // Data is not compressed
                    {
                        destBuffer[destPointer] = sourceBuffer[sourcePointer];
                        sourcePointer++;
                        destPointer++;
                    }
                    else // Data is compressed
                    {
                        int distance, amount;

                        // Let's determine how many bytes the distance & length pair take up
                        switch (sourceBuffer[sourcePointer] >> 4)
                        {
                            case 0: // 3 bytes
                                distance = (((sourceBuffer[sourcePointer + 1] & 0xF) << 8) | sourceBuffer[sourcePointer + 2]) + 1;
                                amount = (((sourceBuffer[sourcePointer] & 0xF) << 4) | (sourceBuffer[sourcePointer + 1] >> 4)) + 17;
                                sourcePointer += 3;
                                break;

                            case 1: // 4 bytes
                                distance = (((sourceBuffer[sourcePointer + 2] & 0xF) << 8) | sourceBuffer[sourcePointer + 3]) + 1;
                                amount = (((sourceBuffer[sourcePointer] & 0xF) << 12) | (sourceBuffer[sourcePointer + 1] << 4) | (sourceBuffer[sourcePointer + 2] >> 4)) + 273;
                                sourcePointer += 4;
                                break;

                            default: // 2 bytes
                                distance = (((sourceBuffer[sourcePointer] & 0xF) << 8) | sourceBuffer[sourcePointer + 1]) + 1;
                                amount = (sourceBuffer[sourcePointer] >> 4) + 1;
                                sourcePointer += 2;
                                break;
                        }

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
            return (length > 4 && PTStream.Contains(source, 0, new byte[] { 0x11 }));
        }
    }
}