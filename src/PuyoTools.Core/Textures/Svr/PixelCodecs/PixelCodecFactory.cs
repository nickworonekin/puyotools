using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.PixelCodecs
{
    internal class PixelCodecFactory
    {
        /// <summary>
        /// Returns a pixel codec for the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The pixel codec, or <see langword="null"/> if one does not exist.</returns>
        public static PixelCodec Create(SvrPixelFormat format) => format switch
        {
            SvrPixelFormat.Rgb5a3 => new Rgb5a3PixelCodec(),
            SvrPixelFormat.Argb8888 => new Argb8888PixelCodec(),
            _ => null,
        };
    }
}
