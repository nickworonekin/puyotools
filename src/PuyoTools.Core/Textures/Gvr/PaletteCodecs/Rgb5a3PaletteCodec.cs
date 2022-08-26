using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PaletteCodecs
{
    /// <inheritdoc/>
    internal class Rgb5a3PaletteCodec : PaletteCodec
    {
        public override int BitsPerPixel => 16;

        public override byte[] Decode(byte[] source)
        {
            var count = source.Length / 2;
            var destination = new byte[count * 4];

            int sourceIndex = 0;
            int destinationIndex = 0;

            for (int i = 0; i < count; i++)
            {
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

                sourceIndex += 2;
                destinationIndex += 4;
            }

            return destination;
        }

        public override byte[] Encode(byte[] source)
        {
            var count = source.Length / 4;
            var destination = new byte[count * 2];

            return Encode(source, destination);
        }

        public override byte[] Encode(byte[] source, byte[] destination)
        {
            var count = source.Length / 4;
            var destinationLength = count * 2;

            if (destination.Length < destinationLength)
            {
                throw new ArgumentException($"Destination must be at least {destination.Length} bytes long.", nameof(destination));
            }

            int sourceIndex = 0;
            int destinationIndex = 0;

            for (int i = 0; i < count; i++)
            {
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

                sourceIndex += 4;
                destinationIndex += 2;
            }

            return destination;
        }
    }
}
