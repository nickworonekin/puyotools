using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.App.Formats.Compression
{
    public static class CompressionFactory
    {
        private static readonly List<ICompressionFormat> decoderFormats;
        private static readonly List<ICompressionFormat> encoderFormats;

        static CompressionFactory()
        {
            // Compression formats that can be used to decompress files.
            decoderFormats = new List<ICompressionFormat>()
            {
                CnxFormat.Instance,
                CompFormat.Instance,
                CxlzFormat.Instance,
                Lz00Format.Instance,
                Lz01Format.Instance,
                Lz10Format.Instance,
                Lz11Format.Instance,
                PrsFormat.Instance,
            };

            // Compression formats that can be used to compress files.
            encoderFormats = new List<ICompressionFormat>()
            {
                CnxFormat.Instance,
                CompFormat.Instance,
                CxlzFormat.Instance,
                Lz00Format.Instance,
                Lz01Format.Instance,
                Lz10Format.Instance,
                Lz11Format.Instance,
                PrsFormat.Instance,
            };
        }

        /// <summary>
        /// Gets the <see cref="ICompressionFormat"/> that describes the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data.</param>
        /// <param name="filename">The name of the file containing the data.</param>
        /// <returns>An instance of <see cref="ICompressionFormat"/>, or null if there is no format.</returns>
        /// <remarks>This method deals with formats used to decompress data. To get all the formats that can be used to compress data, see <see cref="EncoderFormats"/>.</remarks>
        internal static ICompressionFormat GetFormat(Stream source, string filename)
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
        /// Gets a collection of <see cref="ICompressionFormat"/> that can be used to compress data.
        /// </summary>
        internal static IEnumerable<ICompressionFormat> EncoderFormats => encoderFormats.AsReadOnly();
    }
}