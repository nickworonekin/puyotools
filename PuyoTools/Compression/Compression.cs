using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools2.Compression
{
    public static class Compression
    {
        public static Dictionary<CompressionFormat, FormatEntry> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<CompressionFormat, FormatEntry>();

            //Formats.Add(CompressionFormat.CNX, new FormatEntry(null, "CNX", "*.cnx"));
            Formats.Add(CompressionFormat.LZ10, new FormatEntry(new LZ10(), "LZ10", String.Empty));
        }

        public static bool Compress(Stream inStream, string fname, Stream outStream, CompressionFormat format)
        {
            return false;
        }

        public static bool Decompress(Stream inStream, int length, string fname, Stream outStream)
        {
            CompressionFormat format = GetFormat(inStream, length, fname);

            if (format == CompressionFormat.Unknown)
                return false;

            return Formats[format].Class.Decompress(inStream, length, outStream);
        }

        public static bool Decompress(Stream inStream, int length, Stream outStream, CompressionFormat format)
        {
            return Formats[format].Class.Decompress(inStream, length, outStream);
        }

        public static CompressionFormat GetFormat(Stream inStream, int length, string fname)
        {
            foreach (KeyValuePair<CompressionFormat, FormatEntry> format in Formats)
            {
                if (format.Value.Class.Is(inStream, length, fname))
                    return format.Key;
            }

            return CompressionFormat.Unknown;
        }

        public struct FormatEntry
        {
            public CompressionBase Class;
            public string Name;
            public string Filter;

            public FormatEntry(CompressionBase Class, string Name, string Filter)
            {
                this.Class = Class;
                this.Name = Name;
                this.Filter = Filter;
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