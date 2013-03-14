using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools.Archive
{
    public class AFS : ArchiveBase
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
            return (length > 8 && PTStream.Contains(source, 0, new byte[] { (byte)'A', (byte)'F', (byte)'S', 0 }));
        }

        public override bool CanCreate()
        {
            return false;
        }

        public class Read : ArchiveReader
        {
            public Read(Stream source, int length)
            {
                // The start of the archive
                offset = source.Position;

                // Get the number of files in the archive
                source.Position += 4;
                int numFiles = PTStream.ReadInt32(source);
                Files = new ArchiveEntry[numFiles];

                // Get the offset of the metadata
                source.Position += (numFiles * 8);
                int metadataOffset = PTStream.ReadInt32(source);

                // If the offset isn't stored there, then it is stored right before the offset of the first file
                if (metadataOffset == 0)
                {
                    source.Position = offset + 8;
                    source.Position = PTStream.ReadInt32(source) - 8;
                    metadataOffset = PTStream.ReadInt32(source);
                }

                // Read in all the file entries
                for (int i = 0; i < numFiles; i++)
                {
                    // Readin the entry offset and length
                    source.Position = offset + 8 + (i * 8);
                    int entryOffset = PTStream.ReadInt32(source);
                    int entryLength = PTStream.ReadInt32(source);

                    // Read in the entry fname
                    source.Position = metadataOffset + (i * 48);
                    string entryFname = PTStream.ReadCString(source, 32);

                    // Add this entry to the file list
                    Files[i] = new ArchiveEntry(source, offset + entryOffset, entryLength, entryFname);
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
                // The start of the archive
                long offset = destination.Position;
                int blockSize = settings.BlockSize;
                int version = settings.AFSVersion;

                // Magic code "AFS\0"
                destination.WriteByte((byte)'A');
                destination.WriteByte((byte)'F');
                destination.WriteByte((byte)'S');
                destination.WriteByte(0);

                // Number of files in the archive
                PTStream.WriteInt32(destination, files.Count);

                // Write out the header for the archive
                int entryOffset = PTMethods.RoundUp(12 + (files.Count * 8), blockSize);
                int firstEntryOffset = entryOffset;
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteInt32(destination, entryOffset);
                    PTStream.WriteInt32(destination, files[i].Length);

                    entryOffset += PTMethods.RoundUp(files[i].Length, blockSize);
                }

                // If this is AFS v1, then the metadata offset is stored at 8 bytes before
                // the first entry offset.
                if (version == 1)
                {
                    destination.Position = offset + firstEntryOffset - 8;
                }

                // Write out the metadata offset and length
                PTStream.WriteInt32(destination, entryOffset);
                PTStream.WriteInt32(destination, files.Count * 48);

                destination.Position = offset + firstEntryOffset;

                // Write out the file data for each file
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.CopyPartToPadded(files[i].Stream, destination, files[i].Length, blockSize, 0);
                }

                // Write out the footer for the archive
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteCString(destination, files[i].Filename, 32);

                    PTStream.WriteInt16(destination, (short)files[i].Date.Year);
                    PTStream.WriteInt16(destination, (short)files[i].Date.Month);
                    PTStream.WriteInt16(destination, (short)files[i].Date.Day);
                    PTStream.WriteInt16(destination, (short)files[i].Date.Hour);
                    PTStream.WriteInt16(destination, (short)files[i].Date.Minute);
                    PTStream.WriteInt16(destination, (short)files[i].Date.Second);

                    // Write out this data that I have no idea what its purpose is
                    long oldPosition = destination.Position;
                    byte[] buffer = new byte[4];

                    if (version == 1)
                        destination.Position = offset + 8 + (i * 8);
                    else
                        destination.Position = offset + 4 + (i * 4);

                    destination.Read(buffer, 0, 4);
                    destination.Position = oldPosition;
                    destination.Write(buffer, 0, 4);
                }

                // Finish padding out the archive
                while ((destination.Position - offset) % blockSize != 0)
                    destination.WriteByte(0);
            }
        }
    }
}