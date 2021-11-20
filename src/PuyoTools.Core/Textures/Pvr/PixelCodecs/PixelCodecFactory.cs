using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.PixelCodecs
{
    internal static class PixelCodecFactory
    {
        /// <summary>
        /// Returns a pixel codec for the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The pixel codec, or <see langword="null"/> if one does not exist.</returns>
        public static PixelCodec Create(PvrPixelFormat format) => format switch
        {
            PvrPixelFormat.Argb1555 => new Argb1555PixelCodec(),
            PvrPixelFormat.Rgb565 => new Rgb565PixelCodec(),
            PvrPixelFormat.Argb4444 => new Argb4444PixelCodec(),
            PvrPixelFormat.Argb8888 => new Argb8888PixelCodec(),
            _ => null,
        };
    }
}
