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
        public static PixelCodec Create(GimPixelFormat format) => format switch
        {
            GimPixelFormat.Rgb565 => new Rgb565PixelCodec(),
            GimPixelFormat.Argb1555 => new Rgba5551PixelCodec(),
            GimPixelFormat.Argb4444 => new Rgba4444PixelCodec(),
            GimPixelFormat.Argb8888 => new Rgba8888PixelCodec(),
            GimPixelFormat.Index4 => new Index4PixelCodec(),
            GimPixelFormat.Index8 => new Index8PixelCodec(),
            GimPixelFormat.Index16 => new Index16PixelCodec(),
            GimPixelFormat.Index32 => new Index32PixelCodec(),
            GimPixelFormat.Dxt1 or GimPixelFormat.Dxt1Ext => new Dxt1PixelCodec(),
            GimPixelFormat.Dxt3 or GimPixelFormat.Dxt3Ext => new Dxt3PixelCodec(),
            GimPixelFormat.Dxt5 or GimPixelFormat.Dxt5Ext => new Dxt5PixelCodec(),
            _ => null,
        };
    }
}
