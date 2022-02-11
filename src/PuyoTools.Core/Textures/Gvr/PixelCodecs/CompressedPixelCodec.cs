using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class CompressedPixelCodec : PixelCodec
    {
        public override bool CanEncode => false;

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
            throw new NotImplementedException();
        }
    }
}
