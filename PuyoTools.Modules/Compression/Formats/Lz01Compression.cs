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
            while (sourcePointer < sourceLength && destinationPointer < destinationLength)
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

                    // Check to see if we reached the end of the file
                    if (sourcePointer >= sourceLength || destinationPointer >= destinationLength)
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
                PTStream.Contains(source, 0, new byte[] { (byte)'L', (byte)'Z', (byte)'0', (byte)'1' }) &&
                PTStream.ReadInt32At(source, source.Position + 4) == length);
        }
    }
}