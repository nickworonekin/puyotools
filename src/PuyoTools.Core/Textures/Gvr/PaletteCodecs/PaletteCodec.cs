using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PaletteCodecs
{
    internal abstract class PaletteCodec
    {
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
        /// Encodes the specified colors and writes it to the destination.
        /// </summary>
        /// <param name="source">Colors to encode.</param>
        /// <returns></returns>
        public abstract byte[] Encode(byte[] source, byte[] destination);

        /// <summary>
        /// Decodes the specified palette.
        /// </summary>
        /// <param name="source">Palette to decode.</param>
        /// <returns></returns>
        public abstract byte[] Decode(byte[] source);
    }
}
