using System;
using System.IO;

namespace PuyoTools2.Compression
{
    public class LZ10 : CompressionBase
    {
        public override bool Compress(byte[] source, long offset, int length, string fname, Stream destination)
        {
            throw new NotImplementedException();
        }

        public override bool Decompress(byte[] source, long offset, int length, Stream destination)
        {
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

            return true;
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 4 && PTMethods.StreamContains(source, 0, new char['\x10']));
        }

        public override bool CanCompress()
        {
            throw new NotImplementedException();
        }
    }
}