using System;
using System.IO;
using System.Text;

namespace PuyoTools.Modules.Archive
{
    public class TexArchive : ArchiveBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "TEX (Puyo Fever 2)"; }
        }

        /// <summary>
        /// The primary file extension for this archive format.
        /// </summary>
        public override string FileExtension
        {
            get { return ".tex"; }
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
            return new TexArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new TexArchiveWriter(destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && PTStream.Contains(source, 0, new byte[] { (byte)'T', (byte)'E', (byte)'X', (byte)'0' }));
        }
    }

    #region Archive Reader
    public class TexArchiveReader : ArchiveReader
    {
        public TexArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 4;
            int numEntries = PTStream.ReadInt32(source);
            entries = new ArchiveEntryCollection(this, numEntries);

            source.Position += 8;

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry filename extension, offset, length, and filename without the extension
                string entryFileExtension = PTStream.ReadCString(source, 4, Encoding.GetEncoding("Shift_JIS"));
                int entryOffset = PTStream.ReadInt32(source);
                int entryLength = PTStream.ReadInt32(source);
                string entryFilename = PTStream.ReadCString(source, 20, Encoding.GetEncoding("Shift_JIS"));

                if (entryFileExtension != String.Empty)
                    entryFileExtension = "." + entryFileExtension;

                // Add this entry to the collection
                entries.Add(startOffset + entryOffset, entryLength, entryFilename + entryFileExtension);
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class TexArchiveWriter : ArchiveWriter
    {
        public TexArchiveWriter(Stream destination) : base(destination) { }

        public override void Flush()
        {
            // The start of the archive
            long offset = destination.Position;

            // Magic code "TEX0"
            destination.WriteByte((byte)'T');
            destination.WriteByte((byte)'E');
            destination.WriteByte((byte)'X');
            destination.WriteByte((byte)'0');

            // Number of entries in the archive
            PTStream.WriteInt32(destination, entries.Count);

            destination.Position += 8;

            // Write out the header for the archive
            int entryOffset = 16 + (entries.Count * 32);

            for (int i = 0; i < entries.Count; i++)
            {
                // Write out the file extension
                string fileExtension = Path.GetExtension(entries[i].Name);
                if (fileExtension != String.Empty)
                    fileExtension = fileExtension.Substring(1);

                PTStream.WriteCString(destination, fileExtension, 4, Encoding.GetEncoding("Shift_JIS"));

                // Write out the offset, length, and filename (without the extension)
                PTStream.WriteInt32(destination, entryOffset);
                PTStream.WriteInt32(destination, entries[i].Length);
                PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(entries[i].Name), 20, Encoding.GetEncoding("Shift_JIS"));

                entryOffset += PTMethods.RoundUp(entries[i].Length, 16);
            }

            // Write out the file data for each entry
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.CopyToPadded(entries[i].Open(), destination, 16, 0);

                // Call the file added event
                OnFileAdded(EventArgs.Empty);
            }
        }
    }
    #endregion
}