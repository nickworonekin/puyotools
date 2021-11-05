using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PixelCodecs
{
    /// <inheritdoc/>
    internal class Rgba4444PixelCodec : PixelCodec
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

                    destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x0F) * 0xFF / 0x0F);
                    destination[destinationIndex + 2] = (byte)(((pixel >> 0) & 0x0F) * 0xFF / 0x0F);
                    destination[destinationIndex + 1] = (byte)(((pixel >> 4) & 0x0F) * 0xFF / 0x0F);
                    destination[destinationIndex + 0] = (byte)(((pixel >> 8) & 0x0F) * 0xFF / 0x0F);
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
                    pixel |= (ushort)((source[sourceIndex + 3] >> 4) << 12);
                    pixel |= (ushort)((source[sourceIndex + 2] >> 4) << 0);
                    pixel |= (ushort)((source[sourceIndex + 1] >> 4) << 4);
                    pixel |= (ushort)((source[sourceIndex + 0] >> 4) << 8);

                    destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                    destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
                }
            }

            return destination;
        }
    }
}
