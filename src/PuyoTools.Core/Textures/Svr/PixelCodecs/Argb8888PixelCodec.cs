using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.PixelCodecs
{
    /// <inheritdoc/>
    internal class Argb8888PixelCodec : PixelCodec
    {
        public override bool CanEncode => true;

        public override int BitsPerPixel => 32;

        public override byte[] Decode(byte[] source)
        {
            var count = source.Length / 4;
            var destination = new byte[count * 4];

            for (int i = 0; i < count; i++)
            {
                DecodePixel(source, i * 4, destination, i * 4);
            }

            return destination;
        }

        public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
        {
            if ((source[sourceIndex + 3] & 0x80) != 0) // Rgb888
            {
                destination[destinationIndex + 3] = 0xFF;
                destination[destinationIndex + 2] = source[sourceIndex + 0];
                destination[destinationIndex + 1] = source[sourceIndex + 1];
                destination[destinationIndex + 0] = source[sourceIndex + 2];
            }
            else // Argb7888
            {
                destination[destinationIndex + 3] = (byte)((source[sourceIndex + 3] << 1) & 0xFF);
                destination[destinationIndex + 2] = source[sourceIndex + 0];
                destination[destinationIndex + 1] = source[sourceIndex + 1];
                destination[destinationIndex + 0] = source[sourceIndex + 2];
            }
        }

        public override byte[] Encode(byte[] source)
        {
            var count = source.Length / 4;
            var destination = new byte[count * 4];

            for (int i = 0; i < count; i++)
            {
                DecodePixel(source, i * 4, destination, i * 4);
            }

            return destination;
        }

        public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
        {
            if (source[sourceIndex + 3] < 0xFF) // Argb7888
            {
                destination[destinationIndex + 3] = (byte)((source[sourceIndex + 3] >> 1) & 0x7F);
                destination[destinationIndex + 0] = source[sourceIndex + 2];
                destination[destinationIndex + 1] = source[sourceIndex + 1];
                destination[destinationIndex + 2] = source[sourceIndex + 0];
            }
            else // Rgb888
            {
                destination[destinationIndex + 3] = 0x80;
                destination[destinationIndex + 0] = source[sourceIndex + 2];
                destination[destinationIndex + 1] = source[sourceIndex + 1];
                destination[destinationIndex + 2] = source[sourceIndex + 0];
            }
        }
    }
}
