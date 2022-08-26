using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class CompressedPixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 4;

        public override byte[] Decode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height * 4];

            var encodedColors = new ushort[2];
            var colors = new byte[16];

            int sourceIndex = 0;
            int rowIndex;
            int destinationIndex;

            for (int yBlock = 0; yBlock < height; yBlock += 8)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 8)
                {
                    for (int y = 0; y < 8; y += 4)
                    {
                        for (int x = 0; x < 8; x += 4)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                ushort encodedColor = BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(source, sourceIndex + (i * 2)));

                                encodedColors[i] = encodedColor;

                                colors[(i * 4) + 3] = 0xFF;
                                colors[(i * 4) + 2] = (byte)(((encodedColor >> 11) & 0x1F) * 0xFF / 0x1F);
                                colors[(i * 4) + 1] = (byte)(((encodedColor >> 5) & 0x3F) * 0xFF / 0x3F);
                                colors[(i * 4) + 0] = (byte)(((encodedColor >> 0) & 0x1F) * 0xFF / 0x1F);
                            }

                            if (encodedColors[0] > encodedColors[1])
                            {
                                colors[11] = 0xFF;
                                colors[10] = (byte)(((colors[2] * 2) + colors[6]) / 3);
                                colors[9] = (byte)(((colors[1] * 2) + colors[5]) / 3);
                                colors[8] = (byte)(((colors[0] * 2) + colors[4]) / 3);

                                colors[15] = 0xFF;
                                colors[14] = (byte)(((colors[6] * 2) + colors[2]) / 3);
                                colors[13] = (byte)(((colors[5] * 2) + colors[1]) / 3);
                                colors[12] = (byte)(((colors[4] * 2) + colors[0]) / 3);
                            }
                            else
                            {
                                colors[11] = 0xFF;
                                colors[10] = (byte)((colors[2] + colors[6]) / 2);
                                colors[9] = (byte)((colors[1] + colors[5]) / 2);
                                colors[8] = (byte)((colors[0] + colors[4]) / 2);

                                colors[15] = 0x00;
                                colors[14] = 0x00;
                                colors[13] = 0x00;
                                colors[12] = 0x00;
                            }

                            sourceIndex += 4;

                            for (int y2 = 0; y2 < 4 && yBlock + y + y2 < height; y2++)
                            {
                                for (int x2 = 0; x2 < 4 && xBlock + x + x2 < width; x2++)
                                {
                                    int colorIndex = ((source[sourceIndex + y2] >> (6 - (x2 * 2))) & 0x3) * 4;
                                    destinationIndex = (((yBlock + y + y2) * width) + xBlock + x + x2) * 4;

                                    for (int i = 0; i < 4; i++)
                                    {
                                        destination[destinationIndex + i] = colors[colorIndex + i];
                                    }
                                }
                            }

                            sourceIndex += 4;
                        }
                    }
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            var destination = new byte[Math.Max(width * height / 2, 32)];
            var subBlock = new byte[64];

            int sourceIndex;
            int destinationIndex = 0;

            int subBlockStart;
            int subBlockIndex;

            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int yBlock = 0; yBlock < 8; yBlock += 4)
                    {
                        for (int xBlock = 0; xBlock < 8; xBlock += 4)
                        {
                            subBlockStart = 0;

                            for (int ySubBlock = 0; ySubBlock < 4 && y + yBlock + ySubBlock < height; ySubBlock++)
                            {
                                subBlockIndex = subBlockStart;

                                for (int xSubBlock = 0; xSubBlock < 4 && x + xBlock + xSubBlock < width; xSubBlock++)
                                {
                                    sourceIndex = (((y + yBlock + ySubBlock) * width) + x + xBlock + xSubBlock) * 4;

                                    subBlock[subBlockIndex + 3] = source[sourceIndex + 3];
                                    subBlock[subBlockIndex + 2] = source[sourceIndex + 2];
                                    subBlock[subBlockIndex + 1] = source[sourceIndex + 1];
                                    subBlock[subBlockIndex + 0] = source[sourceIndex + 0];

                                    subBlockIndex += 4;
                                }

                                subBlockStart += 16;
                            }

                            ConvertBlockToQuaterCmpr(subBlock).CopyTo(destination, destinationIndex);
                            destinationIndex += 8;
                        }
                    }
                }
            }

            return destination;
        }

        // BC1 methods below from CTools Wii
        private static byte[] ConvertBlockToQuaterCmpr(byte[] block)
        {
            int col1, col2, dist, temp;
            bool alpha;
            byte[][] palette;
            byte[] result;

            dist = col1 = col2 = -1;
            alpha = false;
            result = new byte[8];

            for (int i = 0; i < 15; i++)
            {
                if (block[i * 4 + 3] < 16)
                    alpha = true;
                else
                {
                    for (int j = i + 1; j < 16; j++)
                    {
                        temp = Distance(block, i * 4, block, j * 4);

                        if (temp > dist)
                        {
                            dist = temp;
                            col1 = i;
                            col2 = j;
                        }
                    }
                }
            }

            if (dist == -1)
            {
                palette = new byte[][] { new byte[] { 0, 0, 0, 0xff }, new byte[] { 0xff, 0xff, 0xff, 0xff }, null, null };
            }
            else
            {
                palette = new byte[4][];
                palette[0] = new byte[4];
                palette[1] = new byte[4];

                Array.Copy(block, col1 * 4, palette[0], 0, 3);
                palette[0][3] = 0xff;
                Array.Copy(block, col2 * 4, palette[1], 0, 3);
                palette[1][3] = 0xff;

                if (palette[0][0] >> 3 == palette[1][0] >> 3 && palette[0][1] >> 2 == palette[1][1] >> 2 && palette[0][2] >> 3 == palette[1][2] >> 3)
                    if (palette[0][0] >> 3 == 0 && palette[0][1] >> 2 == 0 && palette[0][2] >> 3 == 0)
                        palette[1][0] = palette[1][1] = palette[1][2] = 0xff;
                    else
                        palette[1][0] = palette[1][1] = palette[1][2] = 0x0;
            }

            result[0] = (byte)(palette[0][2] & 0xf8 | palette[0][1] >> 5);
            result[1] = (byte)(palette[0][1] << 3 & 0xe0 | palette[0][0] >> 3);
            result[2] = (byte)(palette[1][2] & 0xf8 | palette[1][1] >> 5);
            result[3] = (byte)(palette[1][1] << 3 & 0xe0 | palette[1][0] >> 3);

            if ((result[0] > result[2] || (result[0] == result[2] && result[1] >= result[3])) == alpha)
            {
                Array.Copy(result, 0, result, 4, 2);
                Array.Copy(result, 2, result, 0, 2);
                Array.Copy(result, 4, result, 2, 2);

                palette[2] = palette[0];
                palette[0] = palette[1];
                palette[1] = palette[2];
            }

            if (!alpha)
            {
                palette[2] = new byte[] { (byte)(((palette[0][0] << 1) + palette[1][0]) / 3), (byte)(((palette[0][1] << 1) + palette[1][1]) / 3), (byte)(((palette[0][2] << 1) + palette[1][2]) / 3), 0xff };
                palette[3] = new byte[] { (byte)((palette[0][0] + (palette[1][0] << 1)) / 3), (byte)((palette[0][1] + (palette[1][1] << 1)) / 3), (byte)((palette[0][2] + (palette[1][2] << 1)) / 3), 0xff };
            }
            else
            {
                palette[2] = new byte[] { (byte)((palette[0][0] + palette[1][0]) >> 1), (byte)((palette[0][1] + palette[1][1]) >> 1), (byte)((palette[0][2] + palette[1][2]) >> 1), 0xff };
                palette[3] = new byte[] { 0, 0, 0, 0 };
            }

            for (int i = 0; i < block.Length >> 4; i++)
            {
                result[4 + i] = (byte)(LeastDistance(palette, block, i * 16 + 0) << 6 | LeastDistance(palette, block, i * 16 + 4) << 4 | LeastDistance(palette, block, i * 16 + 8) << 2 | LeastDistance(palette, block, i * 16 + 12));
            }

            return result;
        }
        private static int LeastDistance(byte[][] palette, byte[] colour, int offset)
        {
            int dist, best, temp;

            if (colour[offset + 3] < 8)
                return 3;

            dist = int.MaxValue;
            best = 0;

            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i][3] != 0xff)
                    break;

                temp = Distance(palette[i], 0, colour, offset);

                if (temp < dist)
                {
                    if (temp == 0)
                        return i;

                    dist = temp;
                    best = i;
                }
            }

            return best;
        }
        private static int Distance(byte[] colour1, int offset1, byte[] colour2, int offset2)
        {
            int temp, val;

            temp = 0;

            for (int i = 0; i < 3; i++)
            {
                val = colour1[offset1 + i] - colour2[offset2 + i];
                temp += val * val;
            }

            return temp;
        }
    }
}
