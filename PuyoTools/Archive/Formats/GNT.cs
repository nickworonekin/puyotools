using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools2.Archive
{
    public class GNT : ArchiveBase
    {
        public override ArchiveReader Open(Stream source, int length)
        {
            return new Read(source, length);
        }

        public override ArchiveWriter Create(Stream destination, ArchiveWriterSettings settings)
        {
            return new Write(destination, settings);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 36
                && PTStream.Contains(source, 0, new byte[] { (byte)'N', (byte)'G', (byte)'I', (byte)'F' })
                && PTStream.Contains(source, 32, new byte[] { (byte)'N', (byte)'G', (byte)'T', (byte)'L' }));
        }

        public class Read : ArchiveReader
        {
            public Read(Stream source, int length)
            {
                // The start of the archive
                offset = source.Position;

                // Get the number of files in the archive
                source.Position += 48;
                int numFiles = PTStream.ReadInt32BE(source);
                Files = new ArchiveEntry[numFiles];

                source.Position += 8 + (numFiles * 20);

                // Read in all the file entries
                for (int i = 0; i < numFiles; i++)
                {
                    // Readin the entry offset and length
                    int entryLength = PTStream.ReadInt32BE(source);
                    int entryOffset = PTStream.ReadInt32BE(source) + 32;

                    // Add this entry to the file list
                    Files[i] = new ArchiveEntry(source, offset + entryOffset, entryLength, String.Empty);
                }

                // Set the position of the stream to the end of the file
                source.Position = offset + length;
            }
        }

        public class Write : ArchiveWriter
        {
            public Write(Stream destination)
            {
                Initalize(destination, new ArchiveWriterSettings());
            }

            public Write(Stream destination, ArchiveWriterSettings settings)
            {
                Initalize(destination, settings);
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }
        }
    }
}