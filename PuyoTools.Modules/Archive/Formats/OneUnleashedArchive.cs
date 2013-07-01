using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Modules.Archive
{
    public class OneUnleashedArchive : ArchiveBase
    {
        public override string Name
        {
            get { return "ONE (Sonic Unleashed)"; }
        }

        public override string FileExtension
        {
            get { return ".one"; }
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
            throw new NotImplementedException();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 8 && PTStream.Contains(source, 0, new byte[] { (byte)'o', (byte)'n', (byte)'e', (byte)'.' }));
        }

        public class Reader : ArchiveReader
        {
            public Reader(Stream source, int length)
            {
                // The start of the archive
                archiveOffset = source.Position;

                // Get the number of files in the archive
                source.Position += 4;
                int numFiles = PTStream.ReadInt32(source);
                Files = new ArchiveEntry[numFiles];

                // Read in all the file entries
                for (int i = 0; i < numFiles; i++)
                {
                    // Read in the entry filename extension, offset, length, and filename without the extension
                    string entryFilename = PTStream.ReadCString(source, 56);
                    int entryOffset = PTStream.ReadInt32(source);
                    int entryLength = PTStream.ReadInt32(source);

                    // Add this entry to the file list
                    Files[i] = new ArchiveEntry(source, archiveOffset + entryOffset, entryLength, entryFilename);
                }

                // Set the position of the stream to the end of the file
                source.Position = archiveOffset + length;
            }
        }
    }
}