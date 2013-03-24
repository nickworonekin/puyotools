using System;
using System.IO;
using System.Collections.Generic;
using PuyoTools.Archive;

namespace PuyoTools
{
    public static class PTArchive
    {
        public static Dictionary<ArchiveFormat, FormatEntry> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<ArchiveFormat, FormatEntry>();

            Formats.Add(ArchiveFormat.Afs, new FormatEntry(new AfsArchive(), "AFS", ".afs"));
            Formats.Add(ArchiveFormat.Gnt, new FormatEntry(new GntArchive(), "GNT", ".gnt"));
            Formats.Add(ArchiveFormat.Gvm, new FormatEntry(new GvmArchive(), "GVM", ".gvm"));
            Formats.Add(ArchiveFormat.U8,  new FormatEntry(new U8Archive(),  "U8",  ".arc"));
        }

        public static ArchiveWriter Create(Stream outStream, ArchiveFormat format, ArchiveWriterSettings settings)
        {
            return Formats[format].Instance.Create(outStream, settings);
        }

        public static ArchiveReader Open(Stream inStream, int length, string fname)
        {
            ArchiveFormat format = GetFormat(inStream, length, fname);

            if (format == ArchiveFormat.Unknown)
                return null;

            return Formats[format].Instance.Open(inStream, length);
        }

        public static ArchiveReader Open(Stream inStream, int length, ArchiveFormat format)
        {
            return Formats[format].Instance.Open(inStream, length);
        }

        public static ArchiveFormat GetFormat(Stream inStream, int length, string fname)
        {
            foreach (KeyValuePair<ArchiveFormat, FormatEntry> format in Formats)
            {
                if (format.Value.Instance.Is(inStream, length, fname))
                    return format.Key;
            }

            return ArchiveFormat.Unknown;
        }

        public struct FormatEntry
        {
            public readonly ArchiveBase Instance;
            public readonly string Name;
            public readonly string Extension;

            public FormatEntry(ArchiveBase instance, string name, string extension)
            {
                Instance = instance;
                Name = name;
                Extension = extension;
            }
        }
    }

    public enum ArchiveFormat
    {
        Unknown,
        Afs,
        Gnt,
        Gvm,
        U8,
    }
}