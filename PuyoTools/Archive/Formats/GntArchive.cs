using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools.Archive
{
    public class GntArchive : ArchiveBase
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
            return (length > 36
                && PTStream.Contains(source, 0, new byte[] { (byte)'N', (byte)'G', (byte)'I', (byte)'F' })
                && PTStream.Contains(source, 32, new byte[] { (byte)'N', (byte)'G', (byte)'T', (byte)'L' }));
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

                // Magic code "NGIF"
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'G');
                destination.WriteByte((byte)'I');
                destination.WriteByte((byte)'F');

                PTStream.WriteInt32(destination, 24); // Unknown
                PTStream.WriteInt32BE(destination, 1); // Unknown
                PTStream.WriteInt32BE(destination, 32); // Offset of the NGTL chunk?

                // Calculate the size of the NGTL chunk
                int NGTLLength = 0;
                for (int i = 0; i < files.Count; i++)
                {
                    NGTLLength += PTMethods.RoundUp(files[i].Length, 8);
                }

                PTStream.WriteInt32BE(destination, PTMethods.RoundUp(28 + (files.Count * 28), 8) + NGTLLength);
                PTStream.WriteInt32BE(destination, PTMethods.RoundUp(60 + (files.Count * 28), 8) + NGTLLength);
                PTStream.WriteInt32BE(destination, 24 + (files.Count * 4));
                PTStream.WriteInt32BE(destination, 1);

                // NGTL chunk
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'G');
                destination.WriteByte((byte)'T');
                destination.WriteByte((byte)'L');

                PTStream.WriteInt32(destination, PTMethods.RoundUp(20 + (files.Count * 28), 8) + NGTLLength);
                PTStream.WriteInt32BE(destination, 16);
                PTStream.WriteInt32BE(destination, 0);
                PTStream.WriteInt32BE(destination, files.Count);
                PTStream.WriteInt32BE(destination, 28);
                PTStream.WriteInt32BE(destination, 28 + (files.Count * 20));

                // Write out crap bytes
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32BE(destination, 0);
                    PTStream.WriteInt32BE(destination, 0);
                    destination.Write(new byte[] { 0, 1, 0, 1 }, 0, 4);
                    PTStream.WriteInt32BE(destination, i);
                    PTStream.WriteInt32BE(destination, 0);
                }

                // Write out the header for the archive
                int entryOffset = 60 + (files.Count * 28);
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32BE(destination, files[i].Length);
                    PTStream.WriteInt32BE(destination, entryOffset - 32);

                    entryOffset += PTMethods.RoundUp(files[i].Length, 8);
                }

                // Write out the file data for each file
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.CopyPartToPadded(files[i].Stream, destination, files[i].Length, 8, 0);
                }

                // Pad before writing out the NOF0 chunk
                while ((destination.Position - offset) % 8 != 0)
                    destination.WriteByte(0);

                // NOF0 chunk
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'O');
                destination.WriteByte((byte)'F');
                destination.WriteByte((byte)'0');

                // Write out crap bytes
                PTStream.WriteInt32(destination, PTMethods.RoundUp(20 + (files.Count * 4), 8));
                PTStream.WriteInt32BE(destination, files.Count + 2);
                PTStream.WriteInt32BE(destination, 0);
                PTStream.WriteInt32BE(destination, 20);

                // Write out more unknown stuff
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32BE(destination, 32 + (files.Count * 20) + (i * 8));
                }

                PTStream.WriteInt32BE(destination, 24);

                // Pad before we write NEND
                // Finish padding out the archive
                while ((destination.Position - offset) % 16 != 0)
                    destination.WriteByte(0);

                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'E');
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'D');

                while ((destination.Position - offset) % 16 != 0)
                    destination.WriteByte(0);
            }
        }
    }
}