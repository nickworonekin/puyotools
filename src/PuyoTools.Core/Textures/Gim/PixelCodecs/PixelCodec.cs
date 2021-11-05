using PuyoTools.Core.Textures.Gim.PaletteCodecs;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim.PixelCodecs
{
    internal abstract class PixelCodec
    {
        /// <summary>
        /// Gets if this pixel format is supported for encoding.
        /// </summary>
        /// <remarks>If <see langword="false"/>, invoking <see cref="Encode(byte[], int, int, int, int)"/> will throw <see cref="NotSupportedException"/>.</remarks>
        public abstract bool CanEncode { get; }

        /// <summary>
        /// Gets the bits per pixel.
        /// </summary>
        public abstract int BitsPerPixel { get; }

        /// <summary>
        /// Gets the maximum number of entries the palette allows for, or 0 if this pixel format doesn't use a palette.
        /// </summary>
        public virtual int PaletteEntries => 0;

        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        public virtual byte[] Palette { get; set; }

        public abstract byte[] Encode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn);

        public abstract byte[] Decode(byte[] source, int width, int height, int pixelsPerRow, int pixelsPerColumn);
    }
}
