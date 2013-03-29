using System;
using System.IO;
using System.Collections.Generic;
using PuyoTools.Texture;

namespace PuyoTools.Archive
{
    public class PvmArchive : ArchiveBase
    {
        public override ArchiveReader Open(Stream source, int length)
        {
            return new Read(source, length);
        }

        public override ArchiveWriter Create(Stream destination, ArchiveWriterSettings settings)
        {
            return new Write(destination, (settings as PvmWriterSettings) ?? new PvmWriterSettings());
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 12 && PTStream.Contains(source, 0, new byte[] { (byte)'P', (byte)'V', (byte)'M', (byte)'H' }));
        }

        public override bool CanCreate()
        {
            return false;
        }

        public class Read : ArchiveReader
        {
            bool containsFilename, containsPixelFormat, containsDimensions, containsGlobalIndex;
            int tableEntryLength, globalIndexOffset;

            public Read(Stream source, int length)
            {
                // The start of the archive
                archiveOffset = source.Position;

                // The offset of the first entry
                source.Position += 4;
                int entryOffset = PTStream.ReadInt32(source) + 8;
                int headerOffset = 0xC;

                // Read what properties this archive stores for each texture
                byte properties = PTStream.ReadByte(source);
                containsFilename    = (properties & (1 << 3)) > 0;
                containsPixelFormat = (properties & (1 << 2)) > 0;
                containsDimensions  = (properties & (1 << 1)) > 0;
                containsGlobalIndex = (properties & (1 << 0)) > 0;
                source.Position++;

                // Determine the size of each entry in the file table
                tableEntryLength = 2;
                if (containsFilename) tableEntryLength += 28;
                if (containsPixelFormat) tableEntryLength += 2;
                if (containsDimensions) tableEntryLength += 2;

                if (containsGlobalIndex)
                {
                    globalIndexOffset = tableEntryLength;
                    tableEntryLength += 4;
                }

                // Get the number of files in the archive
                ushort numFiles = PTStream.ReadUInt16(source);
                Files = new ArchiveEntry[numFiles];

                // Read in all the file entries
                for (int i = 0; i < numFiles; i++)
                {
                    // We need to need to determine the offset based on the length,
                    // which is stored in the texture data.
                    // We already have the entry offset
                    source.Position = archiveOffset + entryOffset + 4;
                    int entryLength = PTStream.ReadInt32(source) + 8;

                    string entryFname = String.Empty;
                    if (containsFilename)
                    {
                        source.Position = archiveOffset + headerOffset + 2;
                        entryFname = PTStream.ReadCString(source, 28) + ".pvr";
                        headerOffset += tableEntryLength;
                    }

                    // Add this entry to the file list
                    Files[i] = new ArchiveEntry(source, archiveOffset + entryOffset, entryLength, entryFname);

                    entryOffset += entryLength;
                }

                // Set the position of the stream to the end of the file
                source.Position = archiveOffset + length;
            }

            public override ArchiveEntry GetFile(int index)
            {
                // Make sure index is not out of bounds
                if (index < 0 || index > Files.Length)
                    throw new IndexOutOfRangeException();

                long oldPosition = Files[index].Stream.Position;

                MemoryStream data = new MemoryStream();

                // Write out the GBIX header
                data.WriteByte((byte)'G');
                data.WriteByte((byte)'B');
                data.WriteByte((byte)'I');
                data.WriteByte((byte)'X');
                PTStream.WriteInt32(data, 8);

                if (containsGlobalIndex)
                {
                    Files[index].Stream.Position = 0xC + (index * tableEntryLength) + globalIndexOffset;
                    PTStream.WriteInt32(data, PTStream.ReadInt32BE(Files[index].Stream));
                }
                else
                {
                    data.Position += 4;
                }

                data.Position += 4;

                // Now copy over the file data
                Files[index].Stream.Position = Files[index].Offset;
                PTStream.CopyPartTo(Files[index].Stream, data, Files[index].Length);

                Files[index].Stream.Position = oldPosition;
                data.Position = 0;

                return new ArchiveEntry(data, 0, (int)data.Length, Files[index].Filename);
            }
        }

        public class Write : ArchiveWriter
        {
            PvmWriterSettings settings;

            public Write(Stream destination) : this(destination, new PvmWriterSettings()) { }

            public Write(Stream destination, PvmWriterSettings settings)
            {
                Initalize(destination);
                this.settings = settings;
            }

            public override void AddFile(Stream source, int length, string fname)
            {
                // Only GVR textures can be added to a GVM archive.
                // If this is not a GVR texture, throw an exception
                if (!(new PvrTexture()).Is(source, length, fname))
                {
                    throw new CannotAddFileToArchiveException();
                }

                base.AddFile(source, length, fname);
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }
        }
    }

    public partial class PvmWriterSettings : ArchiveWriterSettings
    {
        public bool Filename = true;
        public bool GlobalIndex = true;
        public bool Formats = true;
        public bool Dimensions = true;
    }
}