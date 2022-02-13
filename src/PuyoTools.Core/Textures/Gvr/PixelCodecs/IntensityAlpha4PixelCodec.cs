using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class IntensityAlpha4PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 8;

        public override byte[] Decode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height * 4];

            int sourceIndex = 0;
            int destinationIndex;

            for (int yBlock = 0; yBlock < height; yBlock += 4)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 8)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            destinationIndex = (((yBlock + y) * width) + xBlock + x) * 4;

                            destination[destinationIndex + 3] = (byte)(((source[sourceIndex] >> 4) & 0x0F) * 0xFF / 0x0F);
                            destination[destinationIndex + 2] = (byte)((source[sourceIndex] & 0x0F) * 0xFF / 0x0F);
                            destination[destinationIndex + 1] = (byte)((source[sourceIndex] & 0x0F) * 0xFF / 0x0F);
                            destination[destinationIndex + 0] = (byte)((source[sourceIndex] & 0x0F) * 0xFF / 0x0F);

                            sourceIndex++;
                        }
                    }
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height];

            int sourceIndex;
            int destinationIndex = 0;

            for (int yBlock = 0; yBlock < height; yBlock += 4)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 8)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            sourceIndex = (((yBlock + y) * width) + xBlock + x) * 4;

                            byte pixel = 0x00;
                            pixel |= (byte)((byte)(((0.30 * source[sourceIndex + 2]) + (0.59 * source[sourceIndex + 1]) + (0.11 * source[sourceIndex + 0])) * 0xF / 0xFF) & 0xF);
                            pixel |= (byte)(((source[sourceIndex + 3] * 0xF / 0xFF) & 0xF) << 4);

                            destination[destinationIndex] = pixel;

                            destinationIndex++;
                        }
                    }
                }
            }

            return destination;
        }
    }
}
