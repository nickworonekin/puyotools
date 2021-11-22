using PuyoTools.Core.Textures.Svr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.DataCodecs
{
    /// <inheritdoc/>
    internal class Index4ExternalPaletteDataCodec : DataCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 4;

        public override int PaletteEntries => 16;

        public override bool NeedsExternalPalette => true;

        public Index4ExternalPaletteDataCodec(PixelCodec pixelCodec) : base(pixelCodec)
        {
        }

        public override byte[] Decode(byte[] source, int width, int height)
        {
            if (Palette is null)
            {
                throw new InvalidOperationException("Palette must be set.");
            }

            var destination = new byte[width * height * 4];

            int sourceIndex;
            int destinationIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sourceIndex = ((y * width) + x) / 2;

                    byte paletteIndex = (byte)((source[sourceIndex] >> ((x & 0x1) * 4)) & 0xF);

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
            var destination = new byte[width * height / 2];

            int sourceIndex = 0;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    destinationIndex = ((y * width) + x) / 2;

                    byte paletteIndex = (byte)((source[sourceIndex] & 0xF) << ((x & 0x1) * 4));

                    destination[destinationIndex] |= paletteIndex;

                    sourceIndex++;
                }
            }

            return destination;
        }
    }
}
