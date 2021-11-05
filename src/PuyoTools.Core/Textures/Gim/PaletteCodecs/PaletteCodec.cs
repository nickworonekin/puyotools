using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PaletteCodecs
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
        /// Decodes the specified palette.
        /// </summary>
        /// <param name="source">Palette to decode.</param>
        /// <returns></returns>
        public abstract byte[] Decode(byte[] source);
    }
}
