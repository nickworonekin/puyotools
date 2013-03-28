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

            Formats.Add(ArchiveFormat.Acx, new FormatEntry(new AcxArchive(), null,                    "ACX", ".acx"));
            Formats.Add(ArchiveFormat.Afs, new FormatEntry(new AfsArchive(), new AfsWriterSettings(), "AFS", ".afs"));
            Formats.Add(ArchiveFormat.Gnt, new FormatEntry(new GntArchive(), null,                    "GNT", ".gnt"));
            Formats.Add(ArchiveFormat.Gvm, new FormatEntry(new GvmArchive(), new GvmWriterSettings(), "GVM", ".gvm"));
            Formats.Add(ArchiveFormat.Mrg, new FormatEntry(new MrgArchive(), null,                    "MRG", ".mrg"));
            Formats.Add(ArchiveFormat.Spk, new FormatEntry(new SpkArchive(), null,                    "SPK", ".spk"));
            Formats.Add(ArchiveFormat.Snt, new FormatEntry(new SntArchive(), new SntWriterSettings(), "SNT", ".snt"));
            Formats.Add(ArchiveFormat.Tex, new FormatEntry(new TexArchive(), null,                    "TEX", ".tex"));
            Formats.Add(ArchiveFormat.U8,  new FormatEntry(new U8Archive(),  null,                    "U8",  ".arc"));
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
            public readonly ArchiveWriterSettings SettingsInstance;
            public readonly string Name;
            public readonly string Extension;

            public FormatEntry(ArchiveBase instance, ArchiveWriterSettings settingsInstance, string name, string extension)
            {
                Instance = instance;
                SettingsInstance = settingsInstance;
                Name = name;
                Extension = extension;
            }
        }
    }

    public enum ArchiveFormat
    {
        Unknown,
        Acx,
        Afs,
        Gnt,
        Gvm,
        Mrg,
        Spk,
        Snt,
        Tex,
        U8,
    }
}