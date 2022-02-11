using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Rgba8888PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 32;

        public override byte[] Decode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height * 4];

            int sourceIndex = 0;
            int destinationIndex;

            for (int yBlock = 0; yBlock < height; yBlock += 4)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 4)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            destinationIndex = (((yBlock + y) * width) + xBlock + x) * 4;

                            destination[destinationIndex + 3] = source[sourceIndex + 0];
                            destination[destinationIndex + 2] = source[sourceIndex + 1];
                            destination[destinationIndex + 1] = source[sourceIndex + 32];
                            destination[destinationIndex + 0] = source[sourceIndex + 33];

                            sourceIndex += 2;
                        }
                    }

                    sourceIndex += 32;
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height * 4];

            int sourceIndex;
            int destinationIndex = 0;

            for (int yBlock = 0; yBlock < height; yBlock += 4)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 4)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            sourceIndex = (((yBlock + y) * width) + xBlock + x) * 4;

                            destination[destinationIndex + 0] = source[sourceIndex + 3];
                            destination[destinationIndex + 1] = source[sourceIndex + 2];
                            destination[destinationIndex + 32] = source[sourceIndex + 1];
                            destination[destinationIndex + 33] = source[sourceIndex + 0];

                            destinationIndex += 2;
                        }
                    }

                    destinationIndex += 32;
                }
            }

            return destination;
        }
    }
}
