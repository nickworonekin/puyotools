using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
{
    /// <inheritdoc/>
    internal class VqMipmapsDataCodec : DataCodec
    {
        public override bool CanEncode => false;

        public override int BitsPerPixel => 2;

        public override int PaletteEntries => 1024; // Technically 256

        public override bool HasMipmaps => true;

        public VqMipmapsDataCodec(PixelCodec pixelCodec) : base(pixelCodec)
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
            int destinationIndex;

            if (width == 1 && height == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    destination[i] = Palette[i];
                }

                return destination;
            }

            for (int y = 0; y < height; y += 2)
            {
                for (int x = 0; x < width; x += 2)
                {
                    sourceIndex = (twiddleMap[x / 2] << 1) | twiddleMap[y / 2];

                    int paletteIndex = source[sourceIndex] * 4;

                    for (int x2 = 0; x2 < 2; x2++)
                    {
                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            destinationIndex = (((y + y2) * width) + x + x2) * 4;

                            for (int i = 0; i < 4; i++)
                            {
                                destination[destinationIndex + i] = Palette[(paletteIndex * 4) + i];
                            }

                            paletteIndex++;
                        }
                    }
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            throw new NotSupportedException();
        }

        public override bool IsValidDimensions(int width, int height)
        {
            return width == height
                && width is >= 8 and <= 1024
                && MathHelper.IsPow2(width);
        }
    }
}
