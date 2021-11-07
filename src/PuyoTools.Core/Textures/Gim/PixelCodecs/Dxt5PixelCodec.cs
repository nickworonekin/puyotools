using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PixelCodecs
{
    /// <inheritdoc/>
    internal class Dxt5PixelCodec : PixelCodec
    {
        public override bool CanEncode => false;

        public override int BitsPerPixel => 8;

        public override byte[] Decode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            var destination = new byte[width * height * 4];

            var encodedColors = new ushort[2];
            var colors = new byte[16];
            var alphas = new byte[8];

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

                        colors[(i * 4) + 2] = (byte)(((encodedColor >> 11) & 0x1F) * 0xFF / 0x1F);
                        colors[(i * 4) + 1] = (byte)(((encodedColor >> 5) & 0x3F) * 0xFF / 0x3F);
                        colors[(i * 4) + 0] = (byte)(((encodedColor >> 0) & 0x1F) * 0xFF / 0x1F);
                    }

                    if (encodedColors[0] > encodedColors[1])
                    {
                        colors[10] = (byte)(((colors[2] * 2) + colors[6]) / 3);
                        colors[9] = (byte)(((colors[1] * 2) + colors[5]) / 3);
                        colors[8] = (byte)(((colors[0] * 2) + colors[4]) / 3);

                        colors[14] = (byte)(((colors[6] * 2) + colors[2]) / 3);
                        colors[13] = (byte)(((colors[5] * 2) + colors[1]) / 3);
                        colors[12] = (byte)(((colors[4] * 2) + colors[0]) / 3);
                    }
                    else
                    {
                        colors[10] = (byte)((colors[2] + colors[6]) / 2);
                        colors[9] = (byte)((colors[1] + colors[5]) / 2);
                        colors[8] = (byte)((colors[0] + colors[4]) / 2);

                        colors[14] = 0x00;
                        colors[13] = 0x00;
                        colors[12] = 0x00;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        alphas[i] = source[sourceIndex + rowIndex + 14 + i];
                    }

                    if (alphas[0] > alphas[1])
                    {
                        alphas[2] = (byte)(((alphas[0] * 6) + alphas[1]) / 7);
                        alphas[3] = (byte)(((alphas[0] * 5) + (alphas[1] * 2)) / 7);
                        alphas[4] = (byte)(((alphas[0] * 4) + (alphas[1] * 3)) / 7);
                        alphas[5] = (byte)(((alphas[0] * 3) + (alphas[1] * 4)) / 7);
                        alphas[6] = (byte)(((alphas[0] * 2) + (alphas[1] * 5)) / 7);
                        alphas[7] = (byte)((alphas[0] + (alphas[1] * 6)) / 7);
                    }
                    else
                    {
                        alphas[2] = (byte)(((alphas[0] * 4) + alphas[1]) / 5);
                        alphas[3] = (byte)(((alphas[0] * 3) + alphas[1] * 2) / 5);
                        alphas[4] = (byte)(((alphas[0] * 2) + alphas[1] * 3) / 5);
                        alphas[5] = (byte)((alphas[0] + (alphas[1] * 4)) / 5);
                        alphas[6] = 0;
                        alphas[7] = 0xFF;
                    }

                    ulong alphaIndexes = source[sourceIndex + rowIndex + 8]
                        | (ulong)source[sourceIndex + rowIndex + 9] << 8
                        | (ulong)source[sourceIndex + rowIndex + 10] << 16
                        | (ulong)source[sourceIndex + rowIndex + 11] << 24
                        | (ulong)source[sourceIndex + rowIndex + 12] << 32
                        | (ulong)source[sourceIndex + rowIndex + 13] << 40;

                    for (int y2 = 0; y2 < 4 && y + y2 < height; y2++)
                    {
                        for (int x2 = 0; x2 < 4 && x + x2 < width; x2++)
                        {
                            int colorIndex = ((source[sourceIndex + rowIndex + y2] >> (x2 * 2)) & 0x3) * 4;
                            int alphaIndex = (int)((alphaIndexes >> (((y2 * 4) + x2) * 3)) & 0x7);
                            destinationIndex = (((y + y2) * width) + x + x2) * 4;

                            destination[destinationIndex + 3] = alphas[alphaIndex];
                            for (int i = 0; i < 3; i++)
                            {
                                destination[destinationIndex + i] = colors[colorIndex + i];
                            }
                        }
                    }

                    rowIndex += 16;
                }

                sourceIndex += pixelsPerRow * 4;
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            throw new NotImplementedException();
        }
    }
}
