using PuyoTools.Core.Textures.Gvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr
{
    /// <summary>
    /// A grayscale palette that can be used to decode a GVR texture when the external palette file is not known.
    /// </summary>
    public class GvrGrayscalePalette : GvrPalette
    {
        /// <summary>
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        public override GvrPixelFormat PaletteFormat => throw new NotSupportedException();

        /// <summary>
        /// Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <remarks>To get the palette data, see <see cref="GetPaletteData(PixelCodec)"/>.</remarks>
        public override byte[] GetPaletteData()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a grayscale palette.
        /// </summary>
        /// <param name="dataCodec">The data codec this palette will be used for.</param>
        /// <returns>The palette data as a byte array.</returns>
        internal byte[] GetPaletteData(PixelCodec dataCodec)
        {
            var count = dataCodec.PaletteEntries;
            var palette = new byte[count * 4];

            for (var i = 0; i < count; i++)
            {
                palette[(i * 4) + 3] = 0xFF;
                palette[(i * 4) + 2] = (byte)(i * 0xFF / count);
                palette[(i * 4) + 1] = (byte)(i * 0xFF / count);
                palette[(i * 4) + 0] = (byte)(i * 0xFF / count);
            }

            return palette;
        }
    }
}
