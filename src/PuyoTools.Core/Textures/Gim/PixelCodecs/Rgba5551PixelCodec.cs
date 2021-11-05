using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PixelCodecs
{
    /// <inheritdoc/>
    internal class Rgba5551PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 16;

        public override byte[] Decode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            var destination = new byte[width * height * 4];

            int sourceIndex;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sourceIndex = ((y * pixelsPerRow) + x) * 2;
                    destinationIndex = ((y * width) + x) * 4;

                    ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

                    destination[destinationIndex + 3] = (byte)(((pixel >> 15) & 0x01) * 0xFF);
                    destination[destinationIndex + 2] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
                    destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x1F) * 0xFF / 0x1F);
                    destination[destinationIndex + 0] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn)
        {
            var destination = new byte[pixelsPerRow * pixelsPerColumn * 2];

            int sourceIndex;
            int destinationIndex;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sourceIndex = ((y * width) + x) * 4;
                    destinationIndex = ((y * pixelsPerRow) + x) * 2;

                    ushort pixel = 0x0000;
                    pixel |= (ushort)((source[sourceIndex + 3] >> 7) << 15);
                    pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 0);
                    pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
                    pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 10);

                    destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                    destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
                }
            }

            return destination;
        }
    }
}
