using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
{
    internal static class DataCodecFactory
    {
        /// <summary>
        /// Returns a data codec for the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>The data codec, or <see langword="null"/> if one does not exist.</returns>
        public static DataCodec Create(PvrDataFormat format, PixelCodec pixelCodec) => format switch
        {
            PvrDataFormat.SquareTwiddled => new SquareTwiddledDataCodec(pixelCodec),
            PvrDataFormat.SquareTwiddledMipmaps => new SquareTwiddledMipmapsDataCodec(pixelCodec),
            PvrDataFormat.Vq => new VqDataCodec(pixelCodec),
            PvrDataFormat.VqMipmaps => new VqMipmapsDataCodec(pixelCodec),
            PvrDataFormat.Index4 => new Index4DataCodec(pixelCodec),
            PvrDataFormat.Index4Mipmaps => new Index4MipmapsDataCodec(pixelCodec),
            PvrDataFormat.Index8 => new Index8DataCodec(pixelCodec),
            PvrDataFormat.Index8Mipmaps => new Index8MipmapsDataCodec(pixelCodec),
            PvrDataFormat.Rectangle => new RectangleDataCodec(pixelCodec),
            PvrDataFormat.Stride => new StrideDataCodec(pixelCodec),
            PvrDataFormat.RectangleTwiddled => new RectangleTwiddledDataCodec(pixelCodec),
            PvrDataFormat.SmallVq => new SmallVqDataCodec(pixelCodec),
            PvrDataFormat.SmallVqMipmaps => new SmallVqMipmapsDataCodec(pixelCodec),
            PvrDataFormat.SquareTwiddledMipmapsAlt => new SquareTwiddledMipmapsSa2DataCodec(pixelCodec),
            _ => null,
        };
    }
}
