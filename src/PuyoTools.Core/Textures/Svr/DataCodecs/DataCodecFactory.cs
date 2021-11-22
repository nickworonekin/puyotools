using PuyoTools.Core.Textures.Svr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.DataCodecs
{
    internal class DataCodecFactory
    {
        /// <summary>
        /// Returns a data codec for the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The data codec, or <see langword="null"/> if one does not exist.</returns>
        public static DataCodec Create(SvrDataFormat format, PixelCodec pixelCodec) => format switch
        {
            SvrDataFormat.Rectangle => new DirectColorDataCodec(pixelCodec),
            SvrDataFormat.Index4ExternalPalette => new Index4ExternalPaletteDataCodec(pixelCodec),
            SvrDataFormat.Index8ExternalPalette => new Index8ExternalPaletteDataCodec(pixelCodec),
            SvrDataFormat.Index4Rgb5a3Rectangle
                or SvrDataFormat.Index4Rgb5a3Square
                or SvrDataFormat.Index4Argb8Rectangle
                or SvrDataFormat.Index4Argb8Square => new Index4DataCodec(pixelCodec),
            SvrDataFormat.Index8Rgb5a3Rectangle
                or SvrDataFormat.Index8Rgb5a3Square
                or SvrDataFormat.Index8Argb8Rectangle
                or SvrDataFormat.Index8Argb8Square => new Index8DataCodec(pixelCodec),
            _ => null,
        };
    }
}
