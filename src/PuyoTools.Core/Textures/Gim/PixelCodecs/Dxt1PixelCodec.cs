using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PixelCodecs
{
    /// <inheritdoc/>
    internal class Dxt1PixelCodec : PixelCodec
    {
        public override bool CanEncode => false;

        public override int BitsPerPixel => 4;

        public override byte[] Decode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            var destination = new byte[width * height * 4];

            var encodedColors = new ushort[2];
            var colors = new byte[16];

            int sourceIndex = 0;
            int rowIndex;
            int destinationIndex;

            for (int y = 0; y < height; y += 4)
            {
                rowIndex = 0;

                for (int x = 0; x < width; x += 4)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        ushort encodedColor = BitConverter.ToUInt16(source, sourceIndex + rowIndex + 4 + (i * 2));

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

                    for (int y2 = 0; y2 < 4 && y + y2 < height; y2++)
                    {
                        for (int x2 = 0; x2 < 4 && x + x2 < width; x2++)
                        {
                            int colorIndex = ((source[sourceIndex + rowIndex + y2] >> (x2 * 2)) & 0x3) * 4;
                            destinationIndex = (((y + y2) * width) + x + x2) * 4;

                            for (int i = 0; i < 4; i++)
                            {
                                destination[destinationIndex + i] = colors[colorIndex + i];
                            }
                        }
                    }

                    rowIndex += 8;
                }

                sourceIndex += pixelsPerRow * 2;
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            throw new NotImplementedException();
        }
    }
}
