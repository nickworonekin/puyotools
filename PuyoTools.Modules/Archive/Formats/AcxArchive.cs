using System;
using System.IO;

namespace PuyoTools.Modules.Archive
{
    public class AcxArchive : ArchiveBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "ACX"; }
        }

        /// <summary>
        /// The primary file extension for this archive format.
        /// </summary>
        public override string FileExtension
        {
            get { return ".acx"; }
        }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        public override ArchiveReader Open(Stream source)
        {
            return new AcxArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new AcxArchiveWriter(destination);
        }

        public override ModuleSettingsControl GetModuleSettingsControl()
        {
            return new AcxWriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (Path.GetExtension(fname).ToLower() == ".acx" &&
                length > 8 && PTStream.Contains(source, 0, new byte[] { 0, 0, 0, 0 }));
        }
    }

    #region Archive Reader
    public class AcxArchiveReader : ArchiveReader
    {
        public AcxArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 4;
            int numEntries = PTStream.ReadInt32BE(source);
            entries = new ArchiveEntryCollection(this, numEntries);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry offset and length
                int entryOffset = PTStream.ReadInt32BE(source);
                int entryLength = PTStream.ReadInt32BE(source);

                // Add this entry to the collection
                entries.Add(startOffset + entryOffset, entryLength, String.Empty);
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class AcxArchiveWriter : ArchiveWriter
    {
        #region Settings
        /// <summary>
        /// The block size for this archive. The default value is 4.
        /// </summary>
        public int BlockSize
        {
            get { return blockSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("BlockSize");
                }

                blockSize = value;
            }
        }
        private int blockSize;
        #endregion

        public AcxArchiveWriter(Stream destination) : base(destination)
        {
            // Set default settings
            blockSize = 4;
        }

        public override void Flush()
        {
            // The start of the archive
            long offset = destination.Position;

            // Magic code "\0\0\0\0"
            destination.WriteByte(0);
            destination.WriteByte(0);
            destination.WriteByte(0);
            destination.WriteByte(0);

            // Number of entries in the archive
            PTStream.WriteInt32BE(destination, entries.Count);

            // Write out the header for the archive
            int entryOffset = PTMethods.RoundUp(8 + (entries.Count * 8), blockSize);
            int firstEntryOffset = entryOffset;
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32BE(destination, entryOffset);
                PTStream.WriteInt32BE(destination, entries[i].Length);

                entryOffset += PTMethods.RoundUp(entries[i].Length, blockSize);
            }

            // Pad before writing out the file data
            while ((destination.Position - offset) % blockSize != 0)
                destination.WriteByte(0);

            // Write out the file data for each entry
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.CopyToPadded(entries[i].Open(), destination, blockSize, 0);

                // Call the file added event
                OnFileAdded(EventArgs.Empty);
            }
        }
    }
    #endregion
}