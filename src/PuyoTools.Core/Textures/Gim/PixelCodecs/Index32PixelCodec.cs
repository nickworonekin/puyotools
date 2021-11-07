using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PixelCodecs
{
    /// <inheritdoc/>
    internal class Index32PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 32;

        // Theoretically, the true limit is 4,294,967,296.
        // Since the value that represents the number of entries in the palette is 16 bits, the number of entries are limited to 65,535.
        // Even then, this will be limited to 256 due to limitations with the quantizers utilized.
        // As a result, the maximum number of palette entries will be reported as 256.
        public override int PaletteEntries => 256;

        public override byte[] Decode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            if (Palette is null)
            {
                throw new InvalidOperationException("Palette must be set.");
            }

            var destination = new byte[width * height * 4];

            int sourceIndex;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sourceIndex = ((y * pixelsPerRow) + x) * 4;
                    destinationIndex = ((y * width) + x) * 4;

                    uint paletteIndex = BitConverter.ToUInt32(source, sourceIndex);

                    for (int i = 0; i < 4; i++)
                    {
                        destination[destinationIndex + i] = Palette[(paletteIndex * 4) + i];
                    }
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
                    sourceIndex = (y * width) + x;
                    destinationIndex = ((y * pixelsPerRow) + x) * 4;

                    ushort paletteIndex = source[sourceIndex];

                    destination[destinationIndex + 3] = (byte)((paletteIndex >> 24) & 0xFF);
                    destination[destinationIndex + 2] = (byte)((paletteIndex >> 16) & 0xFF);
                    destination[destinationIndex + 1] = (byte)((paletteIndex >> 8) & 0xFF);
                    destination[destinationIndex + 0] = (byte)(paletteIndex & 0xFF);
                }
            }

            return destination;
        }
    }
}
