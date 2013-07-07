using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools.Modules.Archive
{
    public class AcxArchive : ArchiveBase
    {
        public override string Name
        {
            get { return "ACX"; }
        }

        public override string FileExtension
        {
            get { return ".acx"; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override ArchiveReader Open(Stream source, int length)
        {
            return new Reader(source, length);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new Writer(destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (Path.GetExtension(fname).ToLower() == ".acx" &&
                length > 8 && PTStream.Contains(source, 0, new byte[] { 0, 0, 0, 0 }));
        }

        public class Reader : ArchiveReader
        {
            public Reader(Stream source, int length) : base(source)
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
                    entries.Add(archiveOffset + entryOffset, entryLength, String.Empty);
                }

                // Set the position of the stream to the end of the file
                source.Position = archiveOffset + length;
            }
        }

        public class Writer : ArchiveWriter
        {
            public Writer(Stream destination) : base(destination) { }

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
                int entryOffset = PTMethods.RoundUp(8 + (entries.Count * 8), 4);
                int firstEntryOffset = entryOffset;
                for (int i = 0; i < entries.Count; i++)
                {
                    PTStream.WriteInt32BE(destination, entryOffset);
                    PTStream.WriteInt32BE(destination, entries[i].Length);

                    entryOffset += PTMethods.RoundUp(entries[i].Length, 4);
                }

                // Write out the file data for each entry
                for (int i = 0; i < entries.Count; i++)
                {
                    PTStream.CopyToPadded(entries[i].Open(), destination, 4, 0);

                    // Call the file added event
                    OnFileAdded(EventArgs.Empty);
                }
            }
        }
    }
}