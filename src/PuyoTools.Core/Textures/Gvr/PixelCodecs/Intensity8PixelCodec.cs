using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Intensity8PixelCodec : PixelCodec
    {
        public override bool CanEncode => false;

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

                            destination[destinationIndex + 3] = 0xFF;
                            destination[destinationIndex + 2] = source[sourceIndex];
                            destination[destinationIndex + 1] = source[sourceIndex];
                            destination[destinationIndex + 0] = source[sourceIndex];

                            sourceIndex++;
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
