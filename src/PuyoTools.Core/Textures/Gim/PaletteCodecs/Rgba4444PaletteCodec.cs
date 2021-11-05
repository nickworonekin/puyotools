using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PaletteCodecs
{
    /// <inheritdoc/>
    internal class Rgba4444PaletteCodec : PaletteCodec
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
                ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

                destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 2] = (byte)(((pixel >> 0) & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 4) & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 8) & 0x0F) * 0xFF / 0x0F);

                sourceIndex += 2;
                destinationIndex += 4;
            }

            return destination;
        }

        public override byte[] Encode(byte[] source)
        {
            var count = source.Length / 4;
            var destination = new byte[count * 2];

            int sourceIndex = 0;
            int destinationIndex = 0;

            for (int i = 0; i < count; i++)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)((source[sourceIndex + 3] >> 4) << 12);
                pixel |= (ushort)((source[sourceIndex + 2] >> 4) << 0);
                pixel |= (ushort)((source[sourceIndex + 1] >> 4) << 4);
                pixel |= (ushort)((source[sourceIndex + 0] >> 4) << 8);

                destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                destination[destinationIndex + 0] = (byte)(pixel & 0xFF);

                sourceIndex += 4;
                destinationIndex += 2;
            }

            return destination;
        }
    }
}
