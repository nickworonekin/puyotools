using System;
using System.IO;
using System.Collections.Generic;
using PuyoTools.Compression;

namespace PuyoTools
{
    public static class PTCompression
    {
        public static Dictionary<CompressionFormat, FormatEntry> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<CompressionFormat, FormatEntry>();

            Formats.Add(CompressionFormat.CXLZ, new FormatEntry(new CxlzCompression(), "CXLZ", String.Empty));
            Formats.Add(CompressionFormat.LZ10, new FormatEntry(new Lz10Compression(), "LZ10", String.Empty));
            Formats.Add(CompressionFormat.LZ11, new FormatEntry(new Lz11Compression(), "LZ11", String.Empty));
            Formats.Add(CompressionFormat.PRS,  new FormatEntry(new PrsCompression(),  "PRS",  String.Empty));
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

        public static void Compress(Stream source, Stream destination, string fname, CompressionFormat format)
        {
            //return Formats[format].Class.Compress(
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
            public readonly string Name;
            public readonly string Extension;

            public FormatEntry(CompressionBase instance, string name, string extension)
            {
                Instance = instance;
                Name = name;
                Extension = extension;
            }
        }
    }

    public enum CompressionFormat
    {
        Unknown,
        CNX,
        CXLZ,
        LZ00,
        LZ01,
        LZ10,
        LZ11,
        PRS,
    }
}