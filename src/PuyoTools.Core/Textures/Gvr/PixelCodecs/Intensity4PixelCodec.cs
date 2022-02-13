using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Intensity4PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 4;

        public override byte[] Decode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height * 4];

            int sourceIndex = 0;
            int destinationIndex;

            for (int yBlock = 0; yBlock < height; yBlock += 8)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 8)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            destinationIndex = (((yBlock + y) * width) + xBlock + x) * 4;

                            byte pixel = (byte)((source[sourceIndex / 2] >> ((~x & 0x01) * 4)) & 0x0F);

                            destination[destinationIndex + 3] = 0xFF;
                            destination[destinationIndex + 2] = (byte)(pixel * 0xFF / 0x0F);
                            destination[destinationIndex + 1] = (byte)(pixel * 0xFF / 0x0F);
                            destination[destinationIndex + 0] = (byte)(pixel * 0xFF / 0x0F);

                            sourceIndex++;
                        }
                    }
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height / 2];

            int sourceIndex;
            int destinationIndex = 0;

            for (int yBlock = 0; yBlock < height; yBlock += 8)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 8)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            sourceIndex = (((yBlock + y) * width) + xBlock + x) * 4;

                            byte pixel = (byte)(((0.30 * source[sourceIndex + 2]) + (0.59 * source[sourceIndex + 1]) + (0.11 * source[sourceIndex + 0])) * 0xF / 0xFF);

                            destination[destinationIndex / 2] |= (byte)((pixel & 0xF) << ((~x & 0x1) * 4));

                            destinationIndex++;
                        }
                    }
                }
            }

            return destination;
        }
    }
}
