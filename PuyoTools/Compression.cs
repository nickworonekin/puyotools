using System;
using System.IO;
using System.Collections.Generic;
using PuyoTools.Modules.Compression;

namespace PuyoTools
{
    public static class Compression
    {
        public static Dictionary<CompressionFormat, FormatEntry> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<CompressionFormat, FormatEntry>();

            Formats.Add(CompressionFormat.Cxlz, new FormatEntry(new CxlzCompression()));
            Formats.Add(CompressionFormat.Lz10, new FormatEntry(new Lz10Compression()));
            Formats.Add(CompressionFormat.Lz11, new FormatEntry(new Lz11Compression()));
            Formats.Add(CompressionFormat.Prs,  new FormatEntry(new PrsCompression()));
        }

        public static CompressionFormat Decompress(Stream source, Stream destination, int length, string fname)
        {
            CompressionFormat format = GetFormat(source, length, fname);

            if (format == CompressionFormat.Unknown)
                return format;

            Formats[format].Instance.Decompress(source, destination, length);

            return format;
        }

        public static void Decompress(Stream source, Stream destination, int length, CompressionFormat format)
        {
            Formats[format].Instance.Decompress(source, destination, length);
        }

        public static void Compress(Stream source, Stream destination, int length, string fname, CompressionFormat format)
        {
            Formats[format].Instance.Compress(source, destination, length, fname);
        }

        public static CompressionFormat GetFormat(Stream source, int length, string fname)
        {
            foreach (KeyValuePair<CompressionFormat, FormatEntry> format in Formats)
            {
                if (format.Value.Instance.Is(source, length, fname))
                    return format.Key;
            }

            return CompressionFormat.Unknown;
        }

        public struct FormatEntry
        {
            public readonly CompressionBase Instance;

            public FormatEntry(CompressionBase instance)
            {
                Instance = instance;
            }
        }
    }

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