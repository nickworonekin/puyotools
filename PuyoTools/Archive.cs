using System;
using System.Collections.Generic;
using System.IO;

using PuyoTools.Modules;
using PuyoTools.Modules.Archive;

using PuyoTools.GUI;

namespace PuyoTools
{
    public static class Archive
    {
        // Archive format dictionary
        public static readonly Dictionary<ArchiveFormat, ArchiveBase> Formats;

        // Initalize the archive format dictionary
        static Archive()
        {
            // Initalize the archive format dictionary
            Formats = new Dictionary<ArchiveFormat, ArchiveBase>
            {
                [ArchiveFormat.Acx] = new AcxArchive(),
                [ArchiveFormat.Afs] = new AfsArchive(),
                [ArchiveFormat.Gnt] = new GntArchive(),
                [ArchiveFormat.Gvm] = new GvmArchive(),
                [ArchiveFormat.Mrg] = new MrgArchive(),
                [ArchiveFormat.Narc] = new NarcArchive(),
                [ArchiveFormat.OneUnleashed] = new OneUnleashedArchive(),
                [ArchiveFormat.Pvm] = new PvmArchive(),
                [ArchiveFormat.Snt] = new SntArchive(),
                [ArchiveFormat.Spk] = new SpkArchive(),
                [ArchiveFormat.Tex] = new TexArchive(),
                [ArchiveFormat.U8] = new U8Archive(),
                [ArchiveFormat.OneStorybook] = new OneStorybookArchive(),
                [ArchiveFormat.TxdStorybook] = new TxdStorybookArchive()
            };
        }

        // Opens an archive with the specified archive format.
        public static ArchiveReader Open(Stream source, ArchiveFormat format)
        {
            return Formats[format].Open(source);
        }

        // Creates an archive with the specified archive format and writer settings.
        public static ArchiveWriter Create(Stream source, ArchiveFormat format)
        {
            return Formats[format].Create(source);
        }

        // Returns the archive format used by the source archive.
        public static ArchiveFormat GetFormat(Stream source, string fname)
        {
            foreach (KeyValuePair<ArchiveFormat, ArchiveBase> format in Formats)
            {
                if (format.Value.Is(source, fname))
                    return format.Key;
            }

            return ArchiveFormat.Unknown;
        }

        // Returns the module for this archive format.
        public static ArchiveBase GetModule(ArchiveFormat format)
        {
            return Formats[format];
        }
    }

    // List of archive formats
    public enum ArchiveFormat
    {
        Unknown,
        Acx,
        Afs,
        Gnt,
        Gvm,
        Mrg,
        Narc,
        OneUnleashed,
        Pvm,
        Snt,
        Spk,
        Tex,
        U8,
        OneStorybook,
        TxdStorybook,
        Plugin,
    }
}