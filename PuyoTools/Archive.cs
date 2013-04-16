using System;
using System.IO;
using System.Collections.Generic;
using PuyoTools.Modules.Archive;

using PuyoTools.GUI;

namespace PuyoTools
{
    public static class Archive
    {
        public static Dictionary<ArchiveFormat, FormatEntry> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<ArchiveFormat, FormatEntry>();

            Formats.Add(ArchiveFormat.Acx, new FormatEntry(new AcxArchive(), null));
            Formats.Add(ArchiveFormat.Afs, new FormatEntry(new AfsArchive(), new AfsWriterSettingsGUI()));
            Formats.Add(ArchiveFormat.Gnt, new FormatEntry(new GntArchive(), null));
            Formats.Add(ArchiveFormat.Gvm, new FormatEntry(new GvmArchive(), new GvmWriterSettingsGUI()));
            Formats.Add(ArchiveFormat.Mrg, new FormatEntry(new MrgArchive(), null));
            Formats.Add(ArchiveFormat.Pvm, new FormatEntry(new PvmArchive(), new PvmWriterSettingsGUI()));
            Formats.Add(ArchiveFormat.Spk, new FormatEntry(new SpkArchive(), null));
            Formats.Add(ArchiveFormat.Snt, new FormatEntry(new SntArchive(), new SntWriterSettingsGUI()));
            Formats.Add(ArchiveFormat.Tex, new FormatEntry(new TexArchive(), null));
            Formats.Add(ArchiveFormat.U8,  new FormatEntry(new U8Archive(),  null));
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
            public readonly ArchiveWriterSettingsGUI SettingsInstanceGUI;

            public FormatEntry(ArchiveBase instance, ArchiveWriterSettingsGUI settingsInstanceGUI)
            {
                Instance = instance;
                SettingsInstanceGUI = settingsInstanceGUI;
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
        Pvm,
        Spk,
        Snt,
        Tex,
        U8,
        Plugin,
    }
}