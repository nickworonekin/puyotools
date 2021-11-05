using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PixelCodecs
{
    /// <inheritdoc/>
    internal class Rgba8888PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 32;

        public override byte[] Decode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            var destination = new byte[width * height * 4];

            int sourceIndex;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sourceIndex = ((y * pixelsPerRow) + x) * 4;
                    destinationIndex = ((y * width) + x) * 4;

                    destination[destinationIndex + 3] = source[sourceIndex + 3];
                    destination[destinationIndex + 2] = source[sourceIndex + 0];
                    destination[destinationIndex + 1] = source[sourceIndex + 1];
                    destination[destinationIndex + 0] = source[sourceIndex + 2];
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            var destination = new byte[pixelsPerRow * pixelsPerColumn * 4];

            int sourceIndex;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sourceIndex = ((y * width) + x) * 4;
                    destinationIndex = ((y * pixelsPerRow) + x) * 4;

                    destination[destinationIndex + 3] = source[sourceIndex + 3];
                    destination[destinationIndex + 2] = source[sourceIndex + 0];
                    destination[destinationIndex + 1] = source[sourceIndex + 1];
                    destination[destinationIndex + 0] = source[sourceIndex + 2];
                }
            }

            return destination;
        }
    }
}
