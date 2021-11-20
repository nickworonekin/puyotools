using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
{
    /// <inheritdoc/>
    internal class SquareTwiddledMipmapsSa2DataCodec : DataCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => pixelCodec.BitsPerPixel;

        public override bool HasMipmaps => true;

        public SquareTwiddledMipmapsSa2DataCodec(PixelCodec pixelCodec) : base(pixelCodec)
        {
        }

        public override byte[] Decode(byte[] source, int width, int height)
        {
            int bytesPerPixel = BitsPerPixel / 8;

            int[] twiddleMap = CreateTwiddleMap(width);

            var destination = new byte[width * height * 4];

            int sourceIndex;
            int destinationIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sourceIndex = ((twiddleMap[x] << 1) | twiddleMap[y]) * bytesPerPixel;

                    pixelCodec.DecodePixel(source, sourceIndex, destination, destinationIndex);

                    destinationIndex += 4;
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            int bytesPerPixel = BitsPerPixel / 8;

            int[] twiddleMap = CreateTwiddleMap(width);

            var destination = new byte[width * height * bytesPerPixel];

            int sourceIndex = 0;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    destinationIndex = ((twiddleMap[x] << 1) | twiddleMap[y]) * bytesPerPixel;

                    pixelCodec.EncodePixel(source, sourceIndex, destination, destinationIndex);

                    sourceIndex += 4;
                }
            }

            return destination;
        }

        public override bool IsValidDimensions(int width, int height)
        {
            return width == height
                && width is >= 8 and <= 1024
                && MathHelper.IsPow2(width);
        }
    }
}
