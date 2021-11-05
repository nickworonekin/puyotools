using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PaletteCodecs
{
    internal static class PaletteCodecFactory
    {
        /// <summary>
        /// Returns a palette codec for the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The palette codec, or <see langword="null"/> if one does not exist.</returns>
        public static PaletteCodec Create(GimPaletteFormat format) => format switch
        {
            GimPaletteFormat.Rgb565 => new Rgb565PaletteCodec(),
            GimPaletteFormat.Argb1555 => new Rgba5551PaletteCodec(),
            GimPaletteFormat.Argb4444 => new Rgba4444PaletteCodec(),
            GimPaletteFormat.Argb8888 => new Rgba8888PaletteCodec(),
            _ => null,
        };
    }
}
