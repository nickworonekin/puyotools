using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Index8PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 8;

        public override int PaletteEntries => 256;

        public override byte[] Decode(byte[] source, int width, int height)
        {
            if (Palette is null)
            {
                throw new InvalidOperationException("Palette must be set.");
            }

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

                            byte paletteIndex = source[sourceIndex];

                            for (int i = 0; i < 4; i++)
                            {
                                destination[destinationIndex + i] = Palette[(paletteIndex * 4) + i];
                            }

                            sourceIndex++;
                        }
                    }
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height];

            int sourceIndex;
            int destinationIndex = 0;

            for (int yBlock = 0; yBlock < height; yBlock += 4)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 8)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            sourceIndex = ((yBlock + y) * width) + xBlock + x;

                            destination[destinationIndex] = source[sourceIndex];

                            destinationIndex++;
                        }
                    }
                }
            }

            return destination;
        }
    }
}
