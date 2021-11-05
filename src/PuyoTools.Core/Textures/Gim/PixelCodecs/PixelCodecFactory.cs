using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PixelCodecs
{
    internal static class PixelCodecFactory
    {
        /// <summary>
        /// Returns a pixel codec for the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The pixel codec, or <see langword="null"/> if one does not exist.</returns>
        public static PixelCodec Create(GimDataFormat format) => format switch
        {
            GimDataFormat.Rgb565 => new Rgb565PixelCodec(),
            GimDataFormat.Argb1555 => new Rgba5551PixelCodec(),
            GimDataFormat.Argb4444 => new Rgba4444PixelCodec(),
            GimDataFormat.Argb8888 => new Rgba8888PixelCodec(),
            GimDataFormat.Index4 => new Index4PixelCodec(),
            GimDataFormat.Index8 => new Index8PixelCodec(),
            _ => null,
        };
    }
}
