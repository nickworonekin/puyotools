using System;
using System.IO;
using System.Collections.Generic;
using PuyoTools.Modules;
using PuyoTools.Modules.Archive;

using PuyoTools.GUI;

namespace PuyoTools
{
    public static class Archive
    {
        public static Dictionary<ArchiveFormat, ArchiveBase> Formats;

        public static void Initalize()
        {
            Formats = new Dictionary<ArchiveFormat, ArchiveBase>();

            Formats.Add(ArchiveFormat.Acx, new AcxArchive());
            Formats.Add(ArchiveFormat.Afs, new AfsArchive());
            Formats.Add(ArchiveFormat.Gnt, new GntArchive());
            Formats.Add(ArchiveFormat.Gvm, new GvmArchive());
            Formats.Add(ArchiveFormat.Mrg, new MrgArchive());
            Formats.Add(ArchiveFormat.Pvm, new PvmArchive());
            Formats.Add(ArchiveFormat.Spk, new SpkArchive());
            Formats.Add(ArchiveFormat.Snt, new SntArchive());
            Formats.Add(ArchiveFormat.Tex, new TexArchive());
            Formats.Add(ArchiveFormat.U8,  new U8Archive());
        }

        public static ArchiveWriter Create(Stream outStream, ArchiveFormat format, ModuleWriterSettings settings)
        {
            return Formats[format].Create(outStream, settings);
        }

        public static ArchiveReader Open(Stream inStream, int length, string fname)
        {
            ArchiveFormat format = GetFormat(inStream, length, fname);

            if (format == ArchiveFormat.Unknown)
                return null;

            return Formats[format].Open(inStream, length);
        }

        public static ArchiveReader Open(Stream inStream, int length, ArchiveFormat format)
        {
            return Formats[format].Open(inStream, length);
        }

        public static ArchiveFormat GetFormat(Stream inStream, int length, string fname)
        {
            foreach (KeyValuePair<ArchiveFormat, ArchiveBase> format in Formats)
            {
                if (format.Value.Is(inStream, length, fname))
                    return format.Key;
            }

            return ArchiveFormat.Unknown;
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