using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class Lz00Compression : CompressionBase
    {
        /*
         * LZ00 decompression support by QPjDDYwQLI thanks to the author of ps2dis
         */

        public override string Name
        {
            get { return "LZ00"; }
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

            // Get the source length, destination length, and encryption key
            int sourceLength = PTStream.ReadInt32(source);

            source.Position += 40;

            int destinationLength = PTStream.ReadInt32(source);
            uint key = PTStream.ReadUInt32(source);

            source.Position += 8;

            // Set the source, destination, and buffer pointers
            int sourcePointer      = 0x40;
            int destinationPointer = 0x0;
            int bufferPointer      = 0xFEE;

            // Initalize the buffer
            byte[] buffer = new byte[0x1000];

            // Start decompression
            while (sourcePointer < sourceLength && destinationPointer < destinationLength)
            {
                byte flag = ReadByte(source, ref key);
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x1) != 0) // Not compressed
                    {
                        byte value = ReadByte(source, ref key);
                        sourcePointer++;

                        destination.WriteByte(value);
                        destinationPointer++;

                        buffer[bufferPointer] = value;
                        bufferPointer = (bufferPointer + 1) & 0xFFF;
                    }
                    else // Compressed
                    {
                        byte b1 = ReadByte(source, ref key), b2 = ReadByte(source, ref key);
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
            if (length > 64 && PTStream.Contains(source, 0, new byte[] { (byte)'L', (byte)'Z', (byte)'0', (byte)'0' }))
            {
                source.Position += 4;
                int value = PTStream.ReadInt32(source);
                source.Position -= 8;

                return (value == length);
            }

            return false;
        }

        private byte ReadByte(Stream source, ref uint key)
        {
            return Get(PTStream.ReadByte(source), ref key);
        }

        private void WriteByte(Stream destination, byte value, ref uint key)
        {
            destination.WriteByte(Get(value, ref key));
        }

        private byte Get(byte value, ref uint key)
        {
            // Generate a new key
            uint x = (((((((key << 1) + key) << 5) - key) << 5) + key) << 7) - key;
            x = (x << 6) - x;
            x = (x << 4) - x;

            key = ((x << 2) - x) + 12345;

            // Now return the value since we have the key
            uint t = (key >> 16) & 0x7FFF;
            return (byte)(value ^ ((((t << 8) - t) >> 15)));
        }
    }
}