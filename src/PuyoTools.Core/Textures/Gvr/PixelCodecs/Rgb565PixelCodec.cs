using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Rgb565PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 16;

        public override byte[] Decode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height * 4];

            int blockIndex = 0;
            int sourceIndex;
            int destinationIndex;

            for (int yBlock = 0; yBlock < height; yBlock += 4)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 4)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            sourceIndex = blockIndex + (((y * 4) + x) * 2);
                            destinationIndex = (((yBlock + y) * width) + xBlock + x) * 4;

                            ushort pixel = BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(source, sourceIndex));

                            destination[destinationIndex + 3] = 0xFF;
                            destination[destinationIndex + 2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
                            destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x3F) * 0xFF / 0x3F);
                            destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
                        }
                    }

                    blockIndex += 32;
                }
            }

            return destination;
        }

        public override byte[] Encode(byte[] source, int width, int height)
        {
            var destination = new byte[width * height * 2];

            int blockIndex = 0;
            int sourceIndex;
            int destinationIndex;

            for (int yBlock = 0; yBlock < height; yBlock += 4)
            {
                for (int xBlock = 0; xBlock < width; xBlock += 4)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            sourceIndex = (((yBlock + y) * width) + xBlock + x) * 4;
                            destinationIndex = blockIndex + (((y * 4) + x) * 2);

                            ushort pixel = 0x0000;
                            pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 11);
                            pixel |= (ushort)((source[sourceIndex + 1] >> 2) << 5);
                            pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);

                            destination[destinationIndex + 1] = (byte)(pixel & 0xFF);
                            destination[destinationIndex + 0] = (byte)((pixel >> 8) & 0xFF);
                        }
                    }

                    blockIndex += 32;
                }
            }

            return destination;
        }
    }
}
