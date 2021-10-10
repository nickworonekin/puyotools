using PuyoTools.Core.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Textures
{
    internal partial interface ITextureFormat
    {
        /// <summary>
        /// Gets the name of this texture format.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the primary file extension this texture format uses.
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Gets the primary file extension this texture format's external palette uses, or an empty string if this format doesn't use external palettes.
        /// </summary>
        string PaletteFileExtension { get; }

        /// <summary>
        /// Gets the codec for this texture format.
        /// </summary>
        /// <returns>The texture codec.</returns>
        TextureBase GetCodec();

        /// <summary>
        /// Returns if the codec for this format can decode the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to decompress.</param>
        /// <param name="filename">The name of the file to decompress.</param>
        /// <returns>True if the data can be decoded, false otherwise.</returns>
        bool Identify(Stream source, string filename);
    }
}
