using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PuyoTools.Modules.Archive
{
    public class NarcArchive : ArchiveBase
    {
        public override string Name
        {
            get { return "NARC"; }
        }

        public override string FileExtension
        {
            get { return ".narc"; }
        }

        public override bool CanWrite
        {
            get { return false; }
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
            return (length > 12 &&
                PTStream.Contains(source, 0, new byte[] { (byte)'N', (byte)'A', (byte)'R', (byte)'C', 0xFE, 0xFF, 0x00, 0x01 }) &&
                PTStream.ReadInt32At(source, source.Position + 8) == length);
        }

        public class Reader : ArchiveReader
        {
            public Reader(Stream source, int length) : base(source)
            {
                // Read the archive header
                source.Position += 12;
                ushort fatbOffset = PTStream.ReadUInt16(source); // Should always be 0x10

                // Read the FATB chunk
                source.Position = archiveOffset + fatbOffset + 4;
                uint fntbOffset = PTStream.ReadUInt32(source);
                uint filenameOffset = fntbOffset + 16;

                // Get the number of entries in the archive
                int numEntries = PTStream.ReadInt32(source);
                entries = new ArchiveEntryCollection(this, numEntries);

                // Read the FNTB chunk
                source.Position = archiveOffset + fntbOffset + 4;
                bool hasFilenames = (PTStream.ReadUInt32(source) == 8);

                // Read in all the entries
                source.Position = archiveOffset + fatbOffset + 12;
                for (int i = 0; i < numEntries; i++)
                {
                    // Read the entry offset and length
                    int entryOffset = PTStream.ReadInt32(source);
                    int entryLength = PTStream.ReadInt32(source) - entryOffset;

                    // Read the filename (if it has one)
                    string entryFname = String.Empty;
                    if (hasFilenames)
                    {
                        long oldPosition = source.Position;
                        source.Position = filenameOffset;

                        byte fnameLength = PTStream.ReadByte(source);
                        entryFname = PTStream.ReadCString(source, fnameLength);
                        filenameOffset += (uint)(fnameLength + 1);

                        source.Position -= oldPosition;
                    }

                    // Add this entry to the collection
                    entries.Add(archiveOffset + entryOffset, entryLength, entryFname);
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
                throw new NotImplementedException();
            }
        }
    }
}