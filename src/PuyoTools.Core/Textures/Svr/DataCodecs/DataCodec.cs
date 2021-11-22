using PuyoTools.Core.Textures.Svr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr.DataCodecs
{
    internal abstract class DataCodec
    {
        protected readonly PixelCodec pixelCodec;

        /// <summary>
        /// Gets if this data format is supported for encoding.
        /// </summary>
        /// <remarks>If <see langword="false"/>, invoking <see cref="Encode(byte[], int, int)"/> will throw <see cref="NotSupportedException"/>.</remarks>
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
        /// Gets if this data format requires an external palette file.
        /// </summary>
        public virtual bool NeedsExternalPalette => false;

        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        public virtual byte[] Palette { get; set; }

        protected DataCodec(PixelCodec pixelCodec)
        {
            this.pixelCodec = pixelCodec;
        }

        /// <summary>
        /// Encodes the texture data.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public abstract byte[] Encode(byte[] source, int width, int height);

        /// <summary>
        /// Decodes the texture data.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public abstract byte[] Decode(byte[] source, int width, int height);
    }
}
