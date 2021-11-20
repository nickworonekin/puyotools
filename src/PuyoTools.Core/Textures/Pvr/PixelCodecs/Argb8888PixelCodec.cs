using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.PixelCodecs
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
            destination[destinationIndex + 3] = source[sourceIndex + 3];
            destination[destinationIndex + 2] = source[sourceIndex + 2];
            destination[destinationIndex + 1] = source[sourceIndex + 1];
            destination[destinationIndex + 0] = source[sourceIndex + 0];
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
            destination[destinationIndex + 3] = source[sourceIndex + 3];
            destination[destinationIndex + 2] = source[sourceIndex + 2];
            destination[destinationIndex + 1] = source[sourceIndex + 1];
            destination[destinationIndex + 0] = source[sourceIndex + 0];
        }
    }
}
