using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.PixelCodecs
{
    internal abstract class PixelCodec
    {
        /// <summary>
        /// Gets if this pixel format is supported for encoding.
        /// </summary>
        /// <remarks>If <see langword="false"/>, invoking <see cref="Encode(byte[])"/> or <see cref="EncodePixel(byte[], int, byte[], int)"/> will throw <see cref="NotSupportedException"/>.</remarks>
        public abstract bool CanEncode { get; }

        /// <summary>
        /// Gets the bits per pixel.
        /// </summary>
        public abstract int BitsPerPixel { get; }

        /// <summary>
        /// Encodes the specified colors.
        /// </summary>
        /// <param name="source">Colors to encode.</param>
        /// <returns></returns>
        public abstract byte[] Encode(byte[] source);

        /// <summary>
        /// Decodes the specified palette.
        /// </summary>
        /// <param name="source">Palette to decode.</param>
        /// <returns></returns>
        public abstract byte[] Decode(byte[] source);

        /// <summary>
        /// Encodes the pixel at the specified source index and writes it to the destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destination"></param>
        /// <param name="destinationIndex"></param>
        public abstract void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex);

        /// <summary>
        /// Decodes the pixel at the specified source index and writes it to the destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destination"></param>
        /// <param name="destinationIndex"></param>
        public abstract void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex);
    }
}
