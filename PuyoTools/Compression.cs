using System;
using System.Collections.Generic;
using System.IO;

using PuyoTools.Modules.Compression;

namespace PuyoTools
{
    public static class Compression
    {
        // Compression format dictionary
        public static readonly Dictionary<CompressionFormat, CompressionBase> Formats;

        static Compression()
        {
            // Initalize the compression format dictionary
            Formats = new Dictionary<CompressionFormat, CompressionBase>
            {
                [CompressionFormat.Cnx] = new CnxCompression(),
                [CompressionFormat.Comp] = new CompCompression(),
                [CompressionFormat.Cxlz] = new CxlzCompression(),
                [CompressionFormat.Lz00] = new Lz00Compression(),
                [CompressionFormat.Lz01] = new Lz01Compression(),
                [CompressionFormat.Lz10] = new Lz10Compression(),
                [CompressionFormat.Lz11] = new Lz11Compression(),
                [CompressionFormat.Prs] = new PrsCompression(),
            };
        }

        // Decompress a file when the compression format is not known.
        public static CompressionFormat Decompress(Stream source, Stream destination, string fname)
        {
            CompressionFormat format = GetFormat(source, fname);

            if (format == CompressionFormat.Unknown)
                return format;

            Formats[format].Decompress(source, destination);

            return format;
        }

        // Decompress a file with the specified compression format
        public static void Decompress(Stream source, Stream destination, CompressionFormat format)
        {
            Formats[format].Decompress(source, destination);
        } 

        // Compress a file with the specified compression format
        public static void Compress(Stream source, Stream destination, CompressionFormat format)
        {
            Formats[format].Compress(source, destination);
        }

        // Returns the compression format used by the source file.
        public static CompressionFormat GetFormat(Stream source, string fname)
        {
            foreach (KeyValuePair<CompressionFormat, CompressionBase> format in Formats)
            {
                if (format.Value.Is(source, fname))
                    return format.Key;
            }

            return CompressionFormat.Unknown;
        }

        // Returns the module for this compression format.
        public static CompressionBase GetModule(CompressionFormat format)
        {
            return Formats[format];
        }
    }

    // List of compression formats
    public enum CompressionFormat
    {
        Unknown,
        Cnx,
        Comp,
        Cxlz,
        Lz00,
        Lz01,
        Lz10,
        Lz11,
        Prs,
        Plugin,
    }
}