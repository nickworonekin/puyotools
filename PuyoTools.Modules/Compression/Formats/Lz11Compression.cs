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
            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x4;
            int destinationPointer = 0x0;
            int bufferPointer      = 0x0;

            // Get the destination length
            int destinationLength = PTStream.ReadInt32(source) >> 8;
            if (destinationLength == 0)
            {
                // If the destination length is larger than 0xFFFFFF, then the next 4 bytes
                // is the destination length
                destinationLength = PTStream.ReadInt32(source);
                sourcePointer += 4;
            }

            // Initalize the buffer
            byte[] buffer = new byte[0x1000];

            // Start decompression
            while (sourcePointer < length && destinationPointer < destinationLength)
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

                    // Check to see if we reached the end of the file
                    if (sourcePointer >= length || destinationPointer >= destinationLength)
                        break;

                    flag <<= 1;
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
            return (length > 4 && PTStream.Contains(source, 0, new byte[] { 0x11 }));
        }
    }
}