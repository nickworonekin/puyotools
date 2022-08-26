using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PaletteCodecs
{
    /// <inheritdoc/>
    internal class IntensityAlpha8PaletteCodec : PaletteCodec
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
                destination[destinationIndex + 3] = source[sourceIndex];
                destination[destinationIndex + 2] = source[sourceIndex + 1];
                destination[destinationIndex + 1] = source[sourceIndex + 1];
                destination[destinationIndex + 0] = source[sourceIndex + 1];

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
                destination[destinationIndex + 0] = source[sourceIndex + 3];
                destination[destinationIndex + 1] = (byte)((0.30 * source[sourceIndex + 2]) + (0.59 * source[sourceIndex + 1]) + (0.11 * source[sourceIndex + 0]));

                sourceIndex += 4;
                destinationIndex += 2;
            }

            return destination;
        }
    }
}
