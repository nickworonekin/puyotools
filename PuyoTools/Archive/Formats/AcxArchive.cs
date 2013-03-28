using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools.Archive
{
    public class AcxArchive : ArchiveBase
    {
        public override ArchiveReader Open(Stream source, int length)
        {
            return new Read(source, length);
        }

        public override ArchiveWriter Create(Stream destination, ArchiveWriterSettings settings)
        {
            return new Write(destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (Path.GetExtension(fname).ToLower() == ".acx" &&
                length > 8 && PTStream.Contains(source, 0, new byte[] { 0, 0, 0, 0 }));
        }

        public override bool CanCreate()
        {
            return true;
        }

        public class Read : ArchiveReader
        {
            public Read(Stream source, int length)
            {
                // The start of the archive
                archiveOffset = source.Position;

                // Get the number of files in the archive
                source.Position += 4;
                int numFiles = PTStream.ReadInt32BE(source);
                Files = new ArchiveEntry[numFiles];

                // Read in all the file entries
                for (int i = 0; i < numFiles; i++)
                {
                    // Read in the entry offset and length
                    int entryOffset = PTStream.ReadInt32BE(source);
                    int entryLength = PTStream.ReadInt32BE(source);

                    // Add this entry to the file list
                    Files[i] = new ArchiveEntry(source, archiveOffset + entryOffset, entryLength, String.Empty);
                }

                // Set the position of the stream to the end of the file
                source.Position = archiveOffset + length;
            }
        }

        public class Write : ArchiveWriter
        {
            public Write(Stream destination)
            {
                Initalize(destination);
            }

            public Write(Stream destination, ArchiveWriterSettings settings) : this(destination) { }

            public override void Flush()
            {
                // The start of the archive
                long offset = destination.Position;

                // Magic code "\0\0\0\0"
                destination.WriteByte(0);
                destination.WriteByte(0);
                destination.WriteByte(0);
                destination.WriteByte(0);

                // Number of files in the archive
                PTStream.WriteInt32BE(destination, files.Count);

                // Write out the header for the archive
                int entryOffset = PTMethods.RoundUp(8 + (files.Count * 8), 4);
                int firstEntryOffset = entryOffset;
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32BE(destination, entryOffset);
                    PTStream.WriteInt32BE(destination, files[i].Length);

                    entryOffset += PTMethods.RoundUp(files[i].Length, 4);
                }

                // Write out the file data for each file
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.CopyPartToPadded(files[i].Stream, destination, files[i].Length, 4, 0);
                }
            }
        }
    }
}