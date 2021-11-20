using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Argb1555PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 16;

        public override byte[] Decode(byte[] source)
        {
            var count = source.Length / 2;
            var destination = new byte[count * 4];

            for (int i = 0; i < count; i++)
            {
                DecodePixel(source, i * 2, destination, i * 4);
            }

            return destination;
        }

        public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
        {
            ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

            destination[destinationIndex + 3] = (byte)(((pixel >> 15) & 0x01) * 0xFF);
            destination[destinationIndex + 2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
            destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x1F) * 0xFF / 0x1F);
            destination[destinationIndex + 0] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
        }

        public override byte[] Encode(byte[] source)
        {
            var count = source.Length / 4;
            var destination = new byte[count * 2];

            for (int i = 0; i < count; i++)
            {
                DecodePixel(source, i * 4, destination, i * 2);
            }

            return destination;
        }

        public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
        {
            ushort pixel = 0x0000;
            pixel |= (ushort)((source[sourceIndex + 3] >> 7) << 15);
            pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 10);
            pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
            pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 0);

            destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
            destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
        }
    }
}
