using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures;

namespace PuyoTools.App
{
    public static class TextureFactory
    {
        private static readonly List<ITextureFormat> decoderFormats;
        private static readonly List<ITextureFormat> encoderFormats;

        static TextureFactory()
        {
            // Texture formats that can be used to read textures.
            decoderFormats = new List<ITextureFormat>
            {
                GimFormat.Instance,
                GvrFormat.Instance,
                PvrFormat.Instance,
                SvrFormat.Instance,
            };

            // Texture formats that can be used to write textures.
            encoderFormats = new List<ITextureFormat>
            {
                GimFormat.Instance,
                GvrFormat.Instance,
                PvrFormat.Instance,
                SvrFormat.Instance,
            };
        }

        /// <summary>
        /// Gets the <see cref="ITextureFormat"/> that describes the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data.</param>
        /// <param name="filename">The name of the file containing the data.</param>
        /// <returns>An instance of <see cref="ITextureFormat"/>, or null if there is no format.</returns>
        /// <remarks>This method deals with formats used to read data. To get all the formats that can be used to write data, see <see cref="EncoderFormats"/>.</remarks>
        internal static ITextureFormat GetFormat(Stream source, string filename)
        {
            foreach (var format in decoderFormats)
            {
                if (format.Identify(source, filename))
                {
                    return format;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a collection of <see cref="ITextureFormat"/> that can be used to write texture data.
        /// </summary>
        internal static IEnumerable<ITextureFormat> EncoderFormats => encoderFormats.AsReadOnly();
    }
}