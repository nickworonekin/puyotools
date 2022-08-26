using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PaletteCodecs
{
    /// <inheritdoc/>
    internal class Rgb565PaletteCodec : PaletteCodec
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

                destination[destinationIndex + 3] = 0xFF;
                destination[destinationIndex + 2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x3F) * 0xFF / 0x3F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);

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
                pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 11);
                pixel |= (ushort)((source[sourceIndex + 1] >> 2) << 5);
                pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);

                destination[destinationIndex + 1] = (byte)(pixel & 0xFF);
                destination[destinationIndex + 0] = (byte)((pixel >> 8) & 0xFF);

                sourceIndex += 4;
                destinationIndex += 2;
            }

            return destination;
        }
    }
}
