using System;
using System.Collections.Generic;
using System.IO;

using PuyoTools.Core;
using PuyoTools.Core.Archives;

using System.Linq;

namespace PuyoTools.App.Formats.Archives
{
    public static class ArchiveFactory
    {
        private static readonly List<IArchiveFormat> readerFormats;
        private static readonly List<IArchiveFormat> writerFormats;

        // Initalize the archive format dictionary
        static ArchiveFactory()
        {
            // Archive formats that can be used to read archives.
            readerFormats = new List<IArchiveFormat>
            {
                AcxFormat.Instance,
                AfsFormat.Instance,
                GntFormat.Instance,
                GvmFormat.Instance,
                MrgFormat.Instance,
                NarcFormat.Instance,
                OneStorybookFormat.Instance,
                OneUnleashedFormat.Instance,
                PvmFormat.Instance,
                SntFormat.Instance,
                SpkFormat.Instance,
                SvmFormat.Instance,
                TexFormat.Instance,
                TxdStorybookFormat.Instance,
                U8Format.Instance,
            };

            // Compression formats that can be used to write archives.
            writerFormats = new List<IArchiveFormat>
            {
                AcxFormat.Instance,
                AfsFormat.Instance,
                GntFormat.Instance,
                GvmFormat.Instance,
                MrgFormat.Instance,
                NarcFormat.Instance,
                OneStorybookFormat.Instance,
                OneUnleashedFormat.Instance,
                PvmFormat.Instance,
                SntFormat.Instance,
                SpkFormat.Instance,
                SvmFormat.Instance,
                TexFormat.Instance,
                TxdStorybookFormat.Instance,
                U8Format.Instance,
            };
        }

        /// <summary>
        /// Gets the <see cref="IArchiveFormat"/> that describes the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data.</param>
        /// <param name="filename">The name of the file containing the data.</param>
        /// <returns>An instance of <see cref="IArchiveFormat"/>, or null if there is no format.</returns>
        /// <remarks>This method deals with formats used to read data. To get all the formats that can be used to write data, see <see cref="EncoderFormats"/>.</remarks>
        internal static IArchiveFormat GetFormat(Stream source, string filename)
        {
            foreach (var format in readerFormats)
            {
                if (format.Identify(source, filename))
                {
                    return format;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a collection of <see cref="IArchiveFormat"/> that can be used to write archive data.
        /// </summary>
        internal static IEnumerable<IArchiveFormat> WriterFormats => writerFormats.AsReadOnly();
    }
}