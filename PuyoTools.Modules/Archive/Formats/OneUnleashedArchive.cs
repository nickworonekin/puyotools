using System;
using System.IO;

namespace PuyoTools.Modules.Archive
{
    public class OneUnleashedArchive : ArchiveBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "ONE (Sonic Unleashed)"; }
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
            return new OneUnleashedArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 8 && PTStream.Contains(source, 0, new byte[] { (byte)'o', (byte)'n', (byte)'e', (byte)'.' }));
        }
    }

    #region Archive Reader
    public class OneUnleashedArchiveReader : ArchiveReader
    {
        public OneUnleashedArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 4;
            int numEntries = PTStream.ReadInt32(source);
            entries = new ArchiveEntryCollection(this, numEntries);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry filename, offset, and length
                string entryFilename = PTStream.ReadCString(source, 56);
                int entryOffset = PTStream.ReadInt32(source);
                int entryLength = PTStream.ReadInt32(source);

                // Add this entry to the collection
                entries.Add(startOffset + entryOffset, entryLength, entryFilename);
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion
}