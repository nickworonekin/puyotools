using PuyoTools.Core.Textures.Pvr.PixelCodecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr.DataCodecs
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

        /// <summary>
        /// Gets if this data format has mipmaps.
        /// </summary>
        public abstract bool HasMipmaps { get; }

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
        /// <remarks>When <see cref="IsTwiddled"/> is <see langword="true"/>, the data must be twiddled after invoking this method.</remarks>
        public abstract byte[] Encode(byte[] source, int width, int height);

        /// <summary>
        /// Decodes the texture data.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// <remarks>When <see cref="IsTwiddled"/> is <see langword="true"/>, the data must be un-twiddled before invoking this method.</remarks>
        public abstract byte[] Decode(byte[] source, int width, int height);

        /// <summary>
        /// Gets if the <paramref name="width"/> and <paramref name="height"/> are valid for this format.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public abstract bool IsValidDimensions(int width, int height);

        /// <summary>
        /// Creates and returns the twiddle map for the specified size.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        protected static int[] CreateTwiddleMap(int size)
        {
            int[] twiddleMap = new int[size];

            for (int i = 0; i < size; i++)
            {
                twiddleMap[i] = 0;

                for (int j = 0, k = 1; k <= i; j++, k <<= 1)
                {
                    twiddleMap[i] |= (i & k) << j;
                }
            }

            return twiddleMap;
        }
    }
}
