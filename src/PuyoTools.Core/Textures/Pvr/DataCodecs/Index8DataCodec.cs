using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
{
    /// <inheritdoc/>
    internal class Index8DataCodec : DataCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 8;

        public override int PaletteEntries => 256;

        public override bool NeedsExternalPalette => true;

        public override bool HasMipmaps => false;

        public Index8DataCodec(PixelCodec pixelCodec) : base(pixelCodec)
        {
        }

        public override byte[] Decode(byte[] source, int width, int height)
        {
            if (Palette is null)
            {
                throw new InvalidOperationException("Palette must be set.");
            }

            int size = Math.Min(width, height);

            int[] twiddleMap = CreateTwiddleMap(width);

            var destination = new byte[width * height * 4];

            int sourceIndex;
            int destinationIndex;
            int sourceBlockIndex = 0;

            for (int yStart = 0; yStart < height; yStart += size)
            {
                for (int xStart = 0; xStart < width; xStart += size)
                {
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            sourceIndex = sourceBlockIndex + ((twiddleMap[x] << 1) | twiddleMap[y]);
                            destinationIndex = (((yStart + y) * width) + xStart + x) * 4;

                            byte paletteIndex = source[sourceIndex];

                            for (int i = 0; i < 4; i++)
                            {
                                destination[destinationIndex + i] = Palette[(paletteIndex * 4) + i];
                            }
                        }
                    }

                    sourceBlockIndex += size * size;
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            int size = Math.Min(width, height);

            int[] twiddleMap = CreateTwiddleMap(width);

            var destination = new byte[width * height];

            int sourceIndex;
            int destinationIndex;
            int destinationBlockIndex = 0;

            for (int yStart = 0; yStart < height; yStart += size)
            {
                for (int xStart = 0; xStart < width; xStart += size)
                {
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            sourceIndex = (y * width) + x;
                            destinationIndex = destinationBlockIndex + ((twiddleMap[x] << 1) | twiddleMap[y]);

                            destination[destinationIndex] = source[sourceIndex];
                        }
                    }

                    destinationBlockIndex += size * size;
                }
            }

            return destination;
        }

        public override bool IsValidDimensions(int width, int height)
        {
            return width is >= 8 and <= 1024
                && height is >= 8 and <= 1024
                && MathHelper.IsPow2(width)
                && MathHelper.IsPow2(height);
        }
    }
}
