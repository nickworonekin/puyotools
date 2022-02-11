using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr.PixelCodecs
{
    internal static class PixelCodecFactory
    {
        /// <summary>
        /// Returns a pixel codec for the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The pixel codec, or <see langword="null"/> if one does not exist.</returns>
        public static PixelCodec Create(GvrDataFormat format) => format switch
        {
            GvrDataFormat.Intensity4 => new Intensity4PixelCodec(),
            GvrDataFormat.IntensityA4 => new IntensityAlpha4PixelCodec(),
            GvrDataFormat.Intensity8 => new Intensity8PixelCodec(),
            GvrDataFormat.IntensityA8 => new IntensityAlpha8PixelCodec(),
            GvrDataFormat.Rgb565 => new Rgb565PixelCodec(),
            GvrDataFormat.Rgb5a3 => new Rgb5a3PixelCodec(),
            GvrDataFormat.Argb8888 => new Rgba8888PixelCodec(),
            GvrDataFormat.Index4 => new Index4PixelCodec(),
            GvrDataFormat.Index8 => new Index8PixelCodec(),
            GvrDataFormat.Dxt1 => new CompressedPixelCodec(),
            _ => null,
        };
    }
}
