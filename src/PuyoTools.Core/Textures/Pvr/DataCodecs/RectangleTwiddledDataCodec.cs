using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
{
    /// <inheritdoc/>
    internal class RectangleTwiddledDataCodec : DataCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => pixelCodec.BitsPerPixel;

        public override bool HasMipmaps => false;

        public RectangleTwiddledDataCodec(PixelCodec pixelCodec) : base(pixelCodec)
        {
        }

        public override byte[] Decode(byte[] source, int width, int height)
        {
            int bytesPerPixel = BitsPerPixel / 8;

            int size = Math.Min(width, height);

            int[] twiddleMap = CreateTwiddleMap(size);

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
                            sourceIndex = sourceBlockIndex + (((twiddleMap[x] << 1) | twiddleMap[y]) * bytesPerPixel);
                            destinationIndex = (((yStart + y) * width) + xStart + x) * 4;

                            pixelCodec.DecodePixel(source, sourceIndex, destination, destinationIndex);
                        }
                    }

                    sourceBlockIndex += size * size * bytesPerPixel;
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            int bytesPerPixel = BitsPerPixel / 8;

            int size = Math.Min(width, height);

            int[] twiddleMap = CreateTwiddleMap(size);

            var destination = new byte[width * height * bytesPerPixel];

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
                            sourceIndex = (((y + xStart) * width) + xStart + x) * 4;
                            destinationIndex = destinationBlockIndex + (((twiddleMap[x] << 1) | twiddleMap[y]) * bytesPerPixel);

                            pixelCodec.EncodePixel(source, sourceIndex, destination, destinationIndex);
                        }
                    }

                    destinationBlockIndex += size * bytesPerPixel;
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
