using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Archives
{
    internal partial interface IArchiveFormat
    {
        /// <summary>
        /// Gets the name of this archive format.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the primary file extension this archive format uses.
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Gets the codec for this archive format.
        /// </summary>
        /// <returns>The archive codec.</returns>
        ArchiveBase GetCodec();

        /// <summary>
        /// Returns if the codec for this format can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <param name="filename">The name of the file to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        bool Identify(Stream source, string filename);
    }
}
