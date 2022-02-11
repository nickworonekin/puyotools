using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PaletteCodecs
{
    internal static class PaletteCodecFactory
    {
        /// <summary>
        /// Returns a palette codec for the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The palette codec, or <see langword="null"/> if one does not exist.</returns>
        public static PaletteCodec Create(GvrPixelFormat format) => format switch
        {
            GvrPixelFormat.IntensityA8 => new IntensityAlpha8PaletteCodec(),
            GvrPixelFormat.Rgb565 => new Rgb565PaletteCodec(),
            GvrPixelFormat.Rgb5a3 => new Rgb5a3PaletteCodec(),
            _ => null,
        };
    }
}
