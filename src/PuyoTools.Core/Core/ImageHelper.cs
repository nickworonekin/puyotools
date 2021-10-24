using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PuyoTools.Core
{
    /// <summary>
    /// Contains helper methods for images.
    /// </summary>
    internal class ImageHelper
    {
        public static Image<TPixel> Resize<TPixel>(Image<TPixel> image, int width, int height)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            // If no resizing is necessary, return the image as-is.
            if (image.Width == width && image.Height == height)
            {
                return image;
            }

            var newImage = image.Clone();
            newImage.Mutate(x => x.Resize(width, height));

            return newImage;
        }

        /// <summary>
        /// Builds an exact palette with up to the maximum amount of colors specified in <paramref name="maxColors"/>.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="image"></param>
        /// <param name="maxColors">The maximum number of colors the palette may contain.</param>
        /// <param name="palette">
        /// When this method returns, contains the palette that was created, if a palette was created;
        /// otherwise, <see langword="null"/> if a palette was not created.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns><see langword="true"/> is an exact palette was created; otherwise <see langword="false"/>.</returns>
        public static bool TryBuildExactPalette<TPixel>(Image<TPixel> image, int maxColors, out IList<TPixel> palette)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            palette = null;
            var newPalette = new List<TPixel>(maxColors);

            for (var y = 0; y < image.Height; y++)
            {
                var row = image.GetPixelRowSpan(y);

                for (var x = 0; x < row.Length; x++)
                {
                    if (!newPalette.Contains(row[x]))
                    {
                        // If there are too many colors, then an exact palette cannot be built.
                        if (newPalette.Count == maxColors)
                        {
                            return false;
                        }

                        newPalette.Add(row[x]);
                    }
                }
            }

            palette = newPalette;

            return true;
        }

        /// <summary>
        /// Gets the pixel data for the specified image frame as a byte array.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="imageFrame"></param>
        /// <returns>Pixel data as a byte array.</returns>
        public static byte[] GetPixelDataAsBytes<TPixel>(ImageFrame<TPixel> imageFrame)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            if (!imageFrame.TryGetSinglePixelSpan(out var pixelSpan))
            {
                return MemoryMarshal.AsBytes(pixelSpan).ToArray();
            }

            var data = new TPixel[imageFrame.Width * imageFrame.Height];

            for (var y = 0; y < imageFrame.Height; y++)
            {
                var row = imageFrame.GetPixelRowSpan(y);

                for (var x = 0; x < row.Length; x++)
                {
                    data[(y * imageFrame.Width) + x] = row[x];
                }
            }

            return MemoryMarshal.AsBytes<TPixel>(data).ToArray();
        }

        /// <summary>
        /// Gets the pixel data for the specified indexed image frame as a byte array.
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="imageFrame"></param>
        /// <returns>Pixel data as a byte array.</returns>
        public static byte[] GetPixelDataAsBytes<TPixel>(IndexedImageFrame<TPixel> imageFrame)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var data = new byte[imageFrame.Width * imageFrame.Height];

            for (var y = 0; y < imageFrame.Height; y++)
            {
                var row = imageFrame.GetPixelRowSpan(y);

                for (var x = 0; x < row.Length; x++)
                {
                    data[(y * imageFrame.Width) + x] = row[x];
                }
            }

            return data;
        }
    }
}
