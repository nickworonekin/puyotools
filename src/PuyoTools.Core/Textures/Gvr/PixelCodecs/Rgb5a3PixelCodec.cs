using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Rgb5a3PixelCodec : PixelCodec
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

                            if ((pixel & 0x8000) != 0) // Rgb555
                            {
                                destination[destinationIndex + 3] = 0xFF;
                                destination[destinationIndex + 2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
                                destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x1F) * 0xFF / 0x1F);
                                destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
                            }
                            else // Argb3444
                            {
                                destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x07) * 0xFF / 0x07);
                                destination[destinationIndex + 2] = (byte)(((pixel >> 8) & 0x0F) * 0xFF / 0x0F);
                                destination[destinationIndex + 1] = (byte)(((pixel >> 4) & 0x0F) * 0xFF / 0x0F);
                                destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x0F) * 0xFF / 0x0F);
                            }
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

                            if (source[sourceIndex + 3] <= 0xDA) // Argb3444
                            {
                                pixel |= (ushort)((source[sourceIndex + 3] >> 5) << 12);
                                pixel |= (ushort)((source[sourceIndex + 2] >> 4) << 8);
                                pixel |= (ushort)((source[sourceIndex + 1] >> 4) << 4);
                                pixel |= (ushort)((source[sourceIndex + 0] >> 4) << 0);
                            }
                            else // Rgb555
                            {
                                pixel |= 0x8000;
                                pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 10);
                                pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
                                pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);
                            }

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
