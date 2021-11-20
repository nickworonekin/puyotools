using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
{
    /// <inheritdoc/>
    internal class StrideDataCodec : DataCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => pixelCodec.BitsPerPixel;

        public override bool HasMipmaps => false;

        public StrideDataCodec(PixelCodec pixelCodec) : base(pixelCodec)
        {
        }

        public override byte[] Decode(byte[] source, int width, int height)
        {
            int bytesPerPixel = BitsPerPixel / 8;

            var destination = new byte[width * height * 4];

            int sourceIndex = 0;
            int destinationIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixelCodec.DecodePixel(source, sourceIndex, destination, destinationIndex);

                    sourceIndex += bytesPerPixel;
                    destinationIndex += 4;
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            int bytesPerPixel = BitsPerPixel / 8;

            var destination = new byte[width * height * bytesPerPixel];

            int sourceIndex = 0;
            int destinationIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixelCodec.EncodePixel(source, sourceIndex, destination, destinationIndex);

                    sourceIndex += 4;
                    destinationIndex += bytesPerPixel;
                }
            }

            return destination;
        }

        public override bool IsValidDimensions(int width, int height)
        {
            return width is >= 32 and <= 992
                && height is >= 8 and <= 512
                && width % 32 == 0;
        }
    }
}
