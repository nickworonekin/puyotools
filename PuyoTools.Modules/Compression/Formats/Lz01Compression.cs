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

        public override void Decompress(byte[] source, long offset, Stream destination, int length)
        {
            // Set up information for decompression
            int sourcePointer = 0x10;
            int destPointer   = 0x0;
            int bufferPointer = 0xFEE;

            int sourceLength = (int)(BitConverter.ToUInt32(source, (int)offset + 0x4));
            int destLength   = (int)(BitConverter.ToUInt32(source, (int)offset + 0x8));

            byte[] buffer = new byte[0x1000];

            // Start Decompression
            while (sourcePointer < sourceLength && destPointer < destLength)
            {
                byte flag = source[offset + sourcePointer]; // Compression Flag
                sourcePointer++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flag & 0x1) != 0) // Data is not compressed
                    {
                        buffer[bufferPointer] = source[offset + sourcePointer];
                        destination.WriteByte(buffer[bufferPointer]);
                        sourcePointer++;
                        destPointer++;
                        bufferPointer = (bufferPointer + 1) & 0xFFF;
                    }
                    else // Data is compressed
                    {
                        int bufferOffset = source[offset + sourcePointer] | ((source[offset + sourcePointer + 1] & 0xF0) << 4);
                        int amount = (source[offset + sourcePointer + 1] & 0xF) + 3;
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

        public override void Compress(byte[] source, long offset, Stream destination, int length, string fname)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            if (length > 16 && PTStream.Contains(source, 0, new byte[] { (byte)'L', (byte)'Z', (byte)'0', (byte)'1' }))
            {
                source.Position += 4;

                byte[] buffer = new byte[4];
                source.Read(buffer, 0, 4);

                source.Position -= 8;

                if ((int)BitConverter.ToUInt32(buffer, 0) == length)
                {
                    return true;
                }
            }

            return false;
        }
    }
}