using PuyoTools.Modules.Compression;
using System;
using System.IO;

namespace PuyoTools.Modules.Archive
{
    public class OneStorybookArchive : ArchiveBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "ONE (Sonic and the Secret Rings)"; }
        }

        /// <summary>
        /// The primary file extension for this archive format.
        /// </summary>
        public override string FileExtension
        {
            get { return ".one"; }
        }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        public override ArchiveReader Open(Stream source)
        {
            return new OneStorybookArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            if (PTStream.ReadUInt32BEAt(source, 0x4) != 0x10)
                return false;

            var i = PTStream.ReadUInt32BEAt(source, 0xC);

            if (i != 0xFFFFFFFF && i != 0x00000000)
                return false;

            source.Seek(0, SeekOrigin.Begin);
            return true;
        }
    }

    #region Archive Reader
    public class OneStorybookArchiveReader : ArchiveReader
    {
        private PrsCompression _prsCompression;

        public OneStorybookArchiveReader(Stream source) : base(source)
        {
            _prsCompression = new PrsCompression();

            // Get the number of entries in the archive
            int numEntries = PTStream.ReadInt32BE(source);
            entries = new ArchiveEntryCollection(this, numEntries);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                source.Position = 0x34 + (i * 0x30);
                int entryOffset = PTStream.ReadInt32BE(source);
                int entryLength = PTStream.ReadInt32BE(source);
                string entryFilename = PTStream.ReadCStringAt(source, 0x10 + (i * 0x30), 0x20);

                // Add this entry to the collection
                entries.Add(startOffset + entryOffset, entryLength, entryFilename);
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }

        public override Stream OpenEntry(ArchiveEntry entry)
        {
            archiveData.Seek(entry.Offset, SeekOrigin.Begin);
            var memoryStream = new MemoryStream();
            _prsCompression.Decompress(archiveData, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
    #endregion

    public class OneStorybookArchiveWriter : ArchiveWriter
    {
        private PrsCompression _prsCompression;

        public OneStorybookArchiveWriter(Stream dest) : base(dest)
        {
            _prsCompression = new PrsCompression();
        }

        public override void Flush()
        {

        }
    }
}