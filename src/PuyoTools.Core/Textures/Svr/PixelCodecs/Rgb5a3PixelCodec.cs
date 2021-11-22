using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Rgb5a3PixelCodec : PixelCodec
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

            if ((pixel & 0x8000) != 0) // Rgb555
            {
                destination[destinationIndex + 3] = 0xFF;
                destination[destinationIndex + 2] = (byte)(((pixel >> 0) & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 5) & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
            }
            else // Argb3444
            {
                destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x07) * 0xFF / 0x07);
                destination[destinationIndex + 2] = (byte)(((pixel >> 0) & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 4) & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 8) & 0x0F) * 0xFF / 0x0F);
            }
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

            if (source[sourceIndex + 3] <= 0xDA) // Argb3444
            {
                pixel |= (ushort)((source[sourceIndex + 3] >> 5) << 12);
                pixel |= (ushort)((source[sourceIndex + 2] >> 4) << 0);
                pixel |= (ushort)((source[sourceIndex + 1] >> 4) << 4);
                pixel |= (ushort)((source[sourceIndex + 0] >> 4) << 8);
            }
            else // Rgb555
            {
                pixel |= 0x8000;
                pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 0);
                pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
                pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 10);
            }

            destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
            destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
        }
    }
}
