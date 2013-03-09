using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools2.Archive
{
    public static class Archive
    {
        public static Dictionary<ArchiveFormat, FormatEntry> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<ArchiveFormat, FormatEntry>();

            Formats.Add(ArchiveFormat.AFS, new FormatEntry(new AFS(), "AFS", "*.afs"));
            Formats.Add(ArchiveFormat.GVM, new FormatEntry(new GVM(), "GVM", "*.gvm"));
        }

        public static ArchiveWriter Create(Stream outStream, ArchiveFormat format, ArchiveWriterSettings settings)
        {
            return Formats[format].Class.Create(outStream, settings);
        }

        public static ArchiveReader Open(Stream inStream, int length, string fname)
        {
            ArchiveFormat format = GetFormat(inStream, length, fname);

            if (format == ArchiveFormat.Unknown)
                return null;

            return Formats[format].Class.Open(inStream, length);
        }

        public static ArchiveReader Open(Stream inStream, int length, ArchiveFormat format)
        {
            return Formats[format].Class.Open(inStream, length);
        }

        public static ArchiveFormat GetFormat(Stream inStream, int length, string fname)
        {
            foreach (KeyValuePair<ArchiveFormat, FormatEntry> format in Formats)
            {
                if (format.Value.Class.Is(inStream, length, fname))
                    return format.Key;
            }

            return ArchiveFormat.Unknown;
        }

        public struct FormatEntry
        {
            public ArchiveBase Class;
            public string Name;
            public string Filter;

            public FormatEntry(ArchiveBase Class, string Name, string Filter)
            {
                this.Class = Class;
                this.Name = Name;
                this.Filter = Filter;
            }
        }
    }

    public enum ArchiveFormat
    {
        Unknown,
        AFS,
        GVM,
    }
}