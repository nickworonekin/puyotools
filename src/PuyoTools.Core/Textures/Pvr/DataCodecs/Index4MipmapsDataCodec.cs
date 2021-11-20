using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
{
    /// <inheritdoc/>
    internal class Index4MipmapsDataCodec : DataCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 4;

        public override int PaletteEntries => 16;

        public override bool NeedsExternalPalette => true;

        public override bool HasMipmaps => true;

        public Index4MipmapsDataCodec(PixelCodec pixelCodec) : base(pixelCodec)
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
                    sourceIndex = ((twiddleMap[x] << 1) | twiddleMap[y]) / 2;

                    byte paletteIndex = (byte)((source[sourceIndex] >> ((y & 0x1) * 4)) & 0xF);

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

            var destination = new byte[Math.Max(width * height / 2, 1)];

            if (width == 1 && height == 1)
            {
                destination[0] = (byte)((source[0] & 0xF) << 4);

                return destination;
            }

            int sourceIndex = 0;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    destinationIndex = ((twiddleMap[x] << 1) | twiddleMap[y]) / 2;

                    destination[destinationIndex] |= (byte)((source[sourceIndex] & 0xF) << ((y & 0x1) * 4));

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
