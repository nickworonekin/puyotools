using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
{
    /// <inheritdoc/>
    internal class Index8MipmapsDataCodec : DataCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 8;

        public override int PaletteEntries => 256;

        public override bool NeedsExternalPalette => true;

        public override bool HasMipmaps => true;

        public Index8MipmapsDataCodec(PixelCodec pixelCodec) : base(pixelCodec)
        {
        }

        public override byte[] Decode(byte[] source, int width, int height)
        {
            if (Palette is null)
            {
                throw new InvalidOperationException("Palette must be set.");
            }

            int[] twiddleMap = CreateTwiddleMap(width);

            var destination = new byte[width * height * 4];

            int sourceIndex;
            int destinationIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sourceIndex = (twiddleMap[x] << 1) | twiddleMap[y];

                    byte paletteIndex = source[sourceIndex];

                    for (int i = 0; i < 4; i++)
                    {
                        destination[destinationIndex + i] = Palette[(paletteIndex * 4) + i];
                    }

                    destinationIndex += 4;
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            int[] twiddleMap = CreateTwiddleMap(width);

            var destination = new byte[width * height];

            int sourceIndex = 0;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    destinationIndex = (twiddleMap[x] << 1) | twiddleMap[y];

                    destination[destinationIndex] = source[sourceIndex];

                    sourceIndex++;
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
