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

        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "CNX"; }
        }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Decompress(Stream source, Stream destination)
        {
            source.Position += 8;

            // Get the source length and destination length
            int sourceLength      = PTStream.ReadInt32BE(source) + 16;
            int destinationLength = PTStream.ReadInt32BE(source);

            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x10;
            int destinationPointer = 0x0;
            int bufferPointer      = 0x0;

            // Initalize the buffer
            byte[] buffer = new byte[0x800];

            // Start decompression
            while (sourcePointer < sourceLength)
            {
                byte flag = PTStream.ReadByte(source);
                sourcePointer++;

                for (int i = 0; i < 4; i++)
                {
                    byte value;
                    ushort matchPair;
                    int matchDistance, matchLength;

                    switch (flag & 0x3)
                    {
                        // Jump to the next 0x800 boundary
                        case 0:
                            value = PTStream.ReadByte(source);

                            sourcePointer += value + 1;
                            source.Position += value;

                            i = 3;
                            break;

                        // Not compressed, single byte
                        case 1:
                            value = PTStream.ReadByte(source);
                            sourcePointer++;

                            destination.WriteByte(value);
                            destinationPointer++;

                            buffer[bufferPointer] = value;
                            bufferPointer = (bufferPointer + 1) & 0x7FF;
                            break;

                        // Compressed
                        case 2:
                            matchPair = PTStream.ReadUInt16BE(source);
                            sourcePointer += 2;

                            matchDistance = (matchPair >> 5) + 1;
                            matchLength = (matchPair & 0x1F) + 4;

                            for (int j = 0; j < matchLength; j++)
                            {
                                destination.WriteByte(buffer[(bufferPointer - matchDistance) & 0x7FF]);
                                destinationPointer++;

                                buffer[bufferPointer] = buffer[(bufferPointer - matchDistance) & 0x7FF];
                                bufferPointer = (bufferPointer + 1) & 0x7FF;
                            }
                            break;

                        // Not compressed, multiple bytes
                        case 3:
                            matchLength = PTStream.ReadByte(source);
                            sourcePointer++;

                            for (int j = 0; j < matchLength; j++)
                            {
                                value = PTStream.ReadByte(source);
                                sourcePointer++;

                                destination.WriteByte(value);
                                destinationPointer++;

                                buffer[bufferPointer] = value;
                                bufferPointer = (bufferPointer + 1) & 0x7FF;
                            }
                            break;
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

                    flag >>= 2;
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