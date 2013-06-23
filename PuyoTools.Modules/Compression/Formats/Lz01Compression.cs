using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class Lz01Compression : CompressionBase
    {
        public override string Name
        {
            get { return "LZ01"; }
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
            int bufferPointer = 0xFEE;

            int sourceLength = BitConverter.ToInt32(sourceBuffer, 0x4);
            int destLength   = BitConverter.ToInt32(sourceBuffer, 0x8);

            byte[] buffer = new byte[0x1000];

            // Start Decompression
            while (sourcePointer < sourceLength && destPointer < destLength)
            {
                byte flag = sourceBuffer[sourcePointer]; // Compression Flag
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x1) != 0) // Data is not compressed
                    {
                        buffer[bufferPointer] = sourceBuffer[sourcePointer];
                        destination.WriteByte(buffer[bufferPointer]);
                        sourcePointer++;
                        destPointer++;
                        bufferPointer = (bufferPointer + 1) & 0xFFF;
                    }
                    else // Data is compressed
                    {
                        int bufferOffset = sourceBuffer[sourcePointer] | ((sourceBuffer[sourcePointer + 1] & 0xF0) << 4);
                        int amount = (sourceBuffer[sourcePointer + 1] & 0xF) + 3;
                        sourcePointer += 2;

                        // Copy the data
                        for (int j = 0; j < amount; j++)
                        {
                            buffer[bufferPointer] = buffer[bufferOffset];
                            destination.WriteByte(buffer[bufferPointer]);
                            destPointer++;
                            bufferPointer = (bufferPointer + 1) & 0xFFF;
                            bufferOffset = (bufferOffset + 1) & 0xFFF;
                        }
                    }

                    // Check for out of range
                    if (sourcePointer >= length || destPointer >= destLength)
                        break;

                    flag >>= 1;
                }
            }
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
            if (length > 16 && PTStream.Contains(source, 0, new byte[] { (byte)'L', (byte)'Z', (byte)'0', (byte)'1' }))
            {
                source.Position += 4;
                int value = PTStream.ReadInt32(source);
                source.Position -= 8;

                return (value == length);
            }

            return false;
        }
    }
}