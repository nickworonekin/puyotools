using PuyoTools.Core.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Compression
{
    internal partial interface ICompressionFormat
    {
        /// <summary>
        /// Gets the name of this compression format.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the codec for this compression format.
        /// </summary>
        /// <returns>The compression codec.</returns>
        CompressionBase GetCodec();

        /// <summary>
        /// Returns if the codec for this format can decompress the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to decompress.</param>
        /// <param name="filename">The name of the file to decompress.</param>
        /// <returns>True if the data can be decompressed, false otherwise.</returns>
        bool Identify(Stream source, string filename);
    }
}
