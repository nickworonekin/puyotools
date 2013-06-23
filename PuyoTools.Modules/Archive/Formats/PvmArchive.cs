using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using PuyoTools.Modules.Texture;

namespace PuyoTools.Modules.Archive
{
    public class PvmArchive : ArchiveBase
    {
        public override string Name
        {
            get { return "PVM"; }
        }

        public override string FileExtension
        {
            get { return ".pvm"; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override ArchiveReader Open(Stream source, int length)
        {
            return new Reader(source, length);
        }

        public override ArchiveWriter Create(Stream destination, ModuleWriterSettings settings)
        {
            return new Writer(destination, (settings as WriterSettings) ?? new WriterSettings());
        }

        public override ModuleWriterSettings WriterSettingsObject()
        {
            return new WriterSettings();
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 12 && PTStream.Contains(source, 0, new byte[] { (byte)'P', (byte)'V', (byte)'M', (byte)'H' }));
        }

        public class Reader : ArchiveReader
        {
            bool containsFilename, containsPixelFormat, containsDimensions, containsGlobalIndex;
            int tableEntryLength, globalIndexOffset;

            public Reader(Stream source, int length)
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
                // If this archive does not contain any global indicies, then just return the data as is.
                if (!containsGlobalIndex)
                {
                    return base.GetFile(index);
                }

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

                Files[index].Stream.Position = 0xC + (index * tableEntryLength) + globalIndexOffset;
                PTStream.WriteUInt32(data, PTStream.ReadUInt32(Files[index].Stream));

                data.Position += 4;

                // Now copy over the file data
                Files[index].Stream.Position = Files[index].Offset;
                PTStream.CopyPartTo(Files[index].Stream, data, Files[index].Length);

                Files[index].Stream.Position = oldPosition;
                data.Position = 0;

                return new ArchiveEntry(data, 0, (int)data.Length, Files[index].Filename);
            }
        }

        public class Writer : ArchiveWriter
        {
            WriterSettings settings;

            public Writer(Stream destination) : this(destination, new WriterSettings()) { }

            public Writer(Stream destination, WriterSettings settings)
            {
                Initalize(destination);
                this.settings = settings;
            }

            public override void AddFile(Stream source, int length, string fname)
            {
                // Only PVR textures can be added to a PVM archive.
                // If this is not a PVR texture, throw an exception
                if (!(new PvrTexture()).Is(source, length, fname))
                {
                    throw new CannotAddFileToArchiveException();
                }

                base.AddFile(source, length, fname);
            }

            public override void Flush()
            {
                // Determine the length of each entry in the header
                // and the flags that indicate what is stored in the header
                int entryLength = 2;
                ushort flags = 0;

                if (settings.Filename)
                {
                    entryLength += 28;
                    flags |= 0x8;
                }
                if (settings.Formats)
                {
                    entryLength += 2;
                    flags |= 0x4;
                }
                if (settings.Dimensions)
                {
                    entryLength += 2;
                    flags |= 0x2;
                }
                if (settings.GlobalIndex)
                {
                    entryLength += 4;
                    flags |= 0x1;
                }

                // Write the start of the header
                destination.WriteByte((byte)'P');
                destination.WriteByte((byte)'V');
                destination.WriteByte((byte)'M');
                destination.WriteByte((byte)'H');

                // Offset of the first texture in the archive
                long entryOffset = PTMethods.RoundUp(12 + (files.Count * entryLength), 16);
                PTStream.WriteInt32(destination, (int)entryOffset - 8);

                // Write out the flags
                PTStream.WriteUInt16(destination, flags);

                // Write out the number of files
                PTStream.WriteUInt16(destination, (ushort)files.Count);

                // We're going to be using this a few times. Might as well do this here
                long oldPosition;

                // Now, let's add the files
                for (int i = 0; i < files.Count; i++)
                {
                    // We need to get some information about the texture.
                    // We already checked to make sure this texture is a PVR.
                    // No need to check it again.
                    oldPosition = files[i].Stream.Position;
                    VrSharp.PvrTexture.PvrTexture texture = new VrSharp.PvrTexture.PvrTexture(files[i].Stream, files[i].Length);
                    //VrSharp.PvrTexture.PvrTextureInfo textureInfo = (VrSharp.PvrTexture.PvrTextureInfo)texture.GetTextureInfo();
                    files[i].Stream.Position = oldPosition;

                    // Write out the file number
                    PTStream.WriteUInt16(destination, (ushort)i);

                    // Write the information for this entry in the header
                    if (settings.Filename)
                    {
                        PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(files[i].Filename), 28);
                    }
                    if (settings.Formats)
                    {
                        destination.WriteByte(0);
                        destination.WriteByte((byte)texture.DataFormat);
                    }
                    if (settings.Dimensions)
                    {
                        ushort dimensions = 0;
                        dimensions |= (ushort)(((byte)Math.Log(texture.TextureWidth, 2) - 2) & 0xF);
                        dimensions |= (ushort)((((byte)Math.Log(texture.TextureHeight, 2) - 2) & 0xF) << 4);
                        PTStream.WriteUInt16(destination, dimensions);
                    }
                    if (settings.GlobalIndex)
                    {
                        PTStream.WriteUInt32(destination, texture.GlobalIndex);
                    }

                    // Now write out the file information
                    oldPosition = destination.Position;
                    destination.Position = entryOffset;
                    files[i].Stream.Position += texture.PvrtOffset;

                    PTStream.CopyPartToPadded(files[i].Stream, destination, files[i].Length - texture.PvrtOffset, 16, 0);

                    entryOffset = destination.Position;
                    destination.Position = oldPosition;
                }
            }
        }

        public class WriterSettings : ModuleWriterSettings
        {
            private PvmWriterSettings writerSettingsControls;

            public bool Filename = true;
            public bool GlobalIndex = true;
            public bool Formats = true;
            public bool Dimensions = true;

            public override Control Content()
            {
                writerSettingsControls = new PvmWriterSettings();
                return writerSettingsControls;
            }

            public override void SetSettings()
            {
                Filename = writerSettingsControls.FilenameCheckbox.Checked;
                GlobalIndex = writerSettingsControls.GlobalIndexCheckbox.Checked;
                Formats = writerSettingsControls.FormatCheckbox.Checked;
                Dimensions = writerSettingsControls.DimensionsCheckbox.Checked;
            }
        }
    }
}