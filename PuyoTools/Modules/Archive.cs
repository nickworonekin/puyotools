using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

// Archive Module
namespace PuyoTools.Old
{
    public class Archive
    {
        // The extractor and packer objects
        private ArchiveModule Extractor = null;
        private ArchiveModule Packer    = null;

        private Stream Data     = null;
        private string Filename = null;
        private bool Translate  = false;

        // Restricted variables
        public ArchiveFormat Format { get; private set; }
        public string Name          { get; private set; }
        public string Extension     { get; private set; }

        // Archive Dictionary
        public static Dictionary<ArchiveFormat, ArchiveModule> Dictionary { get; private set; }

        // Set up archive object for extracting
        public Archive(Stream data, string filename)
        {
            // Set up information
            Format    = ArchiveFormat.NULL;
            Name      = null;
            Extension = null;
            Data      = data;
            Filename  = filename;

            // Set up information and initalize extractor
            Data     = data;
            Filename = filename;

            InitalizeExtractor();

            // Translate data if we need to
            if (Translate)
                Data = Extractor.TranslateData(Data) ?? new MemoryStream();
        }

        // Archive object for creation
        public Archive(ArchiveFormat format, string filename)
        {
            // Set up information
            Name      = null;
            Extension = null;
            Filename  = filename;
            Format    = format;

            InitalizePacker();
        }

        // Get file list
        public ArchiveFileList GetFileList()
        {
            return Extractor.GetFileList(Data);
        }

        // Create archive header
        public MemoryStream CreateHeader(string[] files, string[] archiveFilenames, int blockSize, bool[] settings, out uint[] offsetList)
        {
            return Packer.CreateHeader(files, archiveFilenames, blockSize, settings, out offsetList);
        }

        // Create archive footer
        public MemoryStream CreateFooter(string[] files, string[] archiveFilenames, int blockSize, bool[] settings, MemoryStream header)
        {
            return Packer.CreateFooter(files, archiveFilenames, blockSize, settings, header);
        }

        // Format file to add to the archive
        public Stream FormatFileToAdd(Stream data)
        {
            return Packer.FormatFileToAdd(data);
        }

        // Output Directory
        public string OutputDirectory
        {
            get
            {
                return (Name ?? "Archive") + " Extracted";
            }
        }

        // File Extension
        public string FileExtension
        {
            get
            {
                return Extension ?? String.Empty;
            }
        }

        // Padding Byte
        public byte PaddingByte
        {
            get
            {
                return Packer.PaddingByte;
            }
        }

        // Get Data
        public Stream GetData()
        {
            return Data;
        }

        // Initalize Decoder
        private void InitalizeExtractor()
        {
            foreach (KeyValuePair<ArchiveFormat, ArchiveModule> value in Dictionary)
            {
                if (value.Value.Check(Data, Filename))
                {
                    // This is the archive format
                    if (value.Value.CanExtract)
                    {
                        Format    = value.Key;
                        Extractor = value.Value;
                        Name      = Extractor.Name;
                        Extension = Extractor.Extension;
                        Translate = Extractor.Translate;
                    }

                    break;
                }
            }
        }

        // Initalize Packer
        private void InitalizePacker()
        {
            // Get archive creator based on compression format
            if (Dictionary.ContainsKey(Format) && Dictionary[Format].CanPack)
            {
                Packer = Dictionary[Format];
                Name   = Packer.Name;
            }
        }

        // Initalize Archive Dictionary
        public static void InitalizeDictionary()
        {
            Dictionary = new Dictionary<ArchiveFormat, ArchiveModule>();

            // Add all the entries to the dictionary
            Dictionary.Add(ArchiveFormat.ACX,  new ACX());
            Dictionary.Add(ArchiveFormat.AFS,  new AFS());
            Dictionary.Add(ArchiveFormat.GNT,  new GNT());
            Dictionary.Add(ArchiveFormat.GVM,  new GVM());
            Dictionary.Add(ArchiveFormat.MDL,  new MDL());
            Dictionary.Add(ArchiveFormat.MRG,  new MRG());
            Dictionary.Add(ArchiveFormat.NARC, new NARC());
            Dictionary.Add(ArchiveFormat.ONE,  new ONE());
            Dictionary.Add(ArchiveFormat.PVM,  new PVM());
            Dictionary.Add(ArchiveFormat.SBA,  new SBA());
            Dictionary.Add(ArchiveFormat.SNT,  new SNT());
            Dictionary.Add(ArchiveFormat.SPK,  new SPK());
            Dictionary.Add(ArchiveFormat.TEX,  new TEX());
            Dictionary.Add(ArchiveFormat.TXAG, new TXAG());
            Dictionary.Add(ArchiveFormat.VDD,  new VDD());
        }
    }

    public abstract class ArchiveModule
    {
        // Variables
        public string Name      { get; protected set; }
        public string Extension { get; protected set; }
        public bool CanPack     { get; protected set; }
        public bool CanExtract  { get; protected set; }
        public bool Translate   { get; protected set; }
        public string[] Filter  { get; protected set; }
        public byte PaddingByte { get; protected set; }
        public ArchivePackSettings PackSettings { get; protected set; }

        // Archive Functions
        public abstract ArchiveFileList GetFileList(Stream data); // Get Stored Files
        public abstract MemoryStream CreateHeader(string[] files, string[] archiveFilenames, int blockSize, bool[] settings, out uint[] offsetList);
        public virtual MemoryStream CreateFooter(string[] files, string[] archiveFilenames, int blockSize, bool[] settings, MemoryStream header)
        {
            return null;
        }
        public virtual MemoryStream TranslateData(Stream data) // Translate Data
        {
            return null;
        }
        public virtual Stream FormatFileToAdd(Stream data) // Format File to Add
        {
            return data;
        }
        public virtual bool Check(Stream input, string filename)
        {
            return false;
        }
    }

    // Archive Formats
    public enum ArchiveFormat
    {
        NULL, // Unknown Archive Format
        ACX,  // ACX
        AFS,  // AFS
        GNT,  // GNT
        GVM,  // GVM
        MDL,  // MDL
        MRG,  // MRG
        NARC, // NARC
        ONE,  // ONE
        PVM,  // PVM
        SBA,  // Storybook Archive
        SNT,  // SNT
        SPK,  // SPK
        TEX,  // TEX
        TXAG, // TXAG (Sonic Storybook TXD)
        VDD,  // VDD
    }

    // Archive File Header
    public static class ArchiveHeader
    {
        public const string
            NULL = null,
            ACX  = "\x00\x00\x00\x00",
            AFS  = "AFS\x00",
            GVM  = "GVMH",
            MDL  = "\x02\x00\x00\x00",
            MRG  = "MRG0",
            NARC = "NARC",
            NGIF = "NGIF",
            NGTL = "NGTL",
            NSIF = "NSIF",
            NSTL = "NSTL",
            NUIF = "NUIF",
            NUTL = "NUTL",
            ONE  = "one.",
            PVM  = "PVMH",
            SPK  = "SND0",
            TEX  = "TEX0",
            TXAG = "TXAG";
    }

    // This contains the file list of an archive
    public class ArchiveFileList
    {
        public Entry[] Entries { get; private set; }

        public ArchiveFileList(uint entries)
        {
            Entries = new Entry[entries];
        }

        // A file entry in the file list
        public class Entry
        {
            // Set up variables for the file entries
            public string Filename { get; private set; }
            public uint Offset     { get; private set; }
            public uint Length     { get; private set; }

            public Entry(uint offset, uint length, string filename)
            {
                Offset   = offset;
                Length   = length;
                Filename = filename;
            }
        }
    }
}