using System;
using System.Collections.Generic;
using System.IO;

using PuyoTools.Modules.Compression;

namespace PuyoTools
{
    public static class Compression
    {
        // Compression format dictionary
        public static Dictionary<CompressionFormat, CompressionBase> Formats;

        // Initalize the compression format dictionary
        public static void Initalize()
        {
            Formats = new Dictionary<CompressionFormat, CompressionBase>();

            Formats.Add(CompressionFormat.Cnx,  new CnxCompression());
            Formats.Add(CompressionFormat.Cxlz, new CxlzCompression());
            Formats.Add(CompressionFormat.Lz00, new Lz00Compression());
            Formats.Add(CompressionFormat.Lz01, new Lz01Compression());
            Formats.Add(CompressionFormat.Lz10, new Lz10Compression());
            Formats.Add(CompressionFormat.Lz11, new Lz11Compression());
            Formats.Add(CompressionFormat.Prs,  new PrsCompression());
        }

        // Decompress a file when the compression format is not known.
        public static CompressionFormat Decompress(Stream source, Stream destination, int length, string fname)
        {
            CompressionFormat format = GetFormat(source, length, fname);

            if (format == CompressionFormat.Unknown)
                return format;

            Formats[format].Decompress(source, destination, length);

            return format;
        }

        // Decompress a file with the specified compression format
        public static void Decompress(Stream source, Stream destination, int length, CompressionFormat format)
        {
            Formats[format].Decompress(source, destination, length);
        }

        // Compress a file with the specified compression format
        public static void Compress(Stream source, Stream destination, int length, string fname, CompressionFormat format)
        {
            //Formats[format].Compress(source, destination, length, fname);
            Formats[format].Compress(source, destination, length);
        }

        // Returns the compression format used by the source file.
        public static CompressionFormat GetFormat(Stream source, int length, string fname)
        {
            foreach (KeyValuePair<CompressionFormat, CompressionBase> format in Formats)
            {
                if (format.Value.Is(source, length, fname))
                    return format.Key;
            }

            return CompressionFormat.Unknown;
        }
    }

    // List of compression formats
    public enum CompressionFormat
    {
        Unknown,
        Cnx,
        Cxlz,
        Lz00,
        Lz01,
        Lz10,
        Lz11,
        Prs,
        Plugin,
    }
}