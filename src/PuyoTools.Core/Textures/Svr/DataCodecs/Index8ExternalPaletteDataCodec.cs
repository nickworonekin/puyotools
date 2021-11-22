using PuyoTools.Core.Textures.Svr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.DataCodecs
{
    /// <inheritdoc/>
    internal class Index8ExternalPaletteDataCodec : DataCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 8;

        public override int PaletteEntries => 256;

        public override bool NeedsExternalPalette => true;

        public Index8ExternalPaletteDataCodec(PixelCodec pixelCodec) : base(pixelCodec)
        {
        }

        public override byte[] Decode(byte[] source, int width, int height)
        {
            if (Palette is null)
            {
                throw new InvalidOperationException("Palette must be set.");
            }

            var destination = new byte[width * height * 4];

            int sourceIndex = 0;
            int destinationIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Bits 4 and 5 for the palette index are swapped.
                    // Stored as: AAABCAAA
                    // Retireve as: AAACBAAA
                    byte paletteIndex = source[sourceIndex];
                    paletteIndex = (byte)((paletteIndex & 0xE7) | ((paletteIndex & 0x10) >> 1) | ((paletteIndex & 0x08) << 1));

                    for (int i = 0; i < 4; i++)
                    {
                        destination[destinationIndex + i] = Palette[(paletteIndex * 4) + i];
                    }

                    sourceIndex++;
                    destinationIndex += 4;
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height];

            int sourceIndex = 0;
            int destinationIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Bits 4 and 5 for the palette index are swapped.
                    // Stored as: AAABCAAA
                    // Retireve as: AAACBAAA
                    byte paletteIndex = source[sourceIndex];
                    paletteIndex = (byte)((paletteIndex & 0xE7) | ((paletteIndex & 0x10) >> 1) | ((paletteIndex & 0x08) << 1));

                    destination[destinationIndex] = paletteIndex;

                    sourceIndex++;
                    destinationIndex++;
                }
            }

            return destination;
        }
    }
}
