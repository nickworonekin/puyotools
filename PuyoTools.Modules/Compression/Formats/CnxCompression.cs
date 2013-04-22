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

        public override void Decompress(byte[] source, long offset, Stream destination, int length)
        {
            // Set up information for decompression
            int sourcePointer = 0x10;
            int destPointer   = 0x0;

            int sourceLength = (int)(PTMethods.ToUInt32BE(source, (int)offset + 0x8) + 16);
            int destLength   = (int)(PTMethods.ToUInt32BE(source, (int)offset + 0xC));

            byte[] destBuffer = new byte[destLength];

            // Start Decompression
            while (sourcePointer < length && destPointer < destLength)
            {
                byte flag = source[offset + sourcePointer]; // Compression Flag
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
                            sourcePointer += (source[offset + sourcePointer] + 1);
                            i = 3;
                            break;

                        // Data is not compressed
                        // Single byte copy mode
                        case 1:
                            destBuffer[destPointer] = source[offset + sourcePointer];
                            sourcePointer++;
                            destPointer++;
                            break;

                        // Data is compressed
                        case 2:
                            value = PTMethods.ToUInt16BE(source, sourcePointer);
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
                            amount = source[sourcePointer];
                            sourcePointer++;

                            // Copy the data
                            for (int j = 0; j < amount; j++)
                            {
                                destBuffer[destPointer] = source[sourcePointer];
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

        public override void Compress(byte[] source, long offset, Stream destination, int length, string fname)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            if (length > 16 && PTStream.Contains(source, 0, new byte[] { (byte)'C', (byte)'N', (byte)'X', 0x2 }))
            {
                source.Position += 8;

                byte[] buffer = new byte[4];
                source.Read(buffer, 0, 4);

                source.Position -= 12;

                if ((int)PTMethods.ToUInt32BE(buffer, 0) + 16 == length)
                {
                    return true;
                }
            }

            return false;
        }
    }
}