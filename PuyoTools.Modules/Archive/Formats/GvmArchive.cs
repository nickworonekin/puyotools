using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using PuyoTools.Modules.Texture;

namespace PuyoTools.Modules.Archive
{
    public class GvmArchive : ArchiveBase
    {
        public override string Name
        {
            get { return "GVM"; }
        }

        public override string FileExtension
        {
            get { return ".gvm"; }
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
            return (length > 12 && PTStream.Contains(source, 0, new byte[] { (byte)'G', (byte)'V', (byte)'M', (byte)'H' }));
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
                source.Position++;
                byte properties = PTStream.ReadByte(source);
                containsFilename    = (properties & (1 << 3)) > 0;
                containsPixelFormat = (properties & (1 << 2)) > 0;
                containsDimensions  = (properties & (1 << 1)) > 0;
                containsGlobalIndex = (properties & (1 << 0)) > 0;

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
                ushort numFiles = PTStream.ReadUInt16BE(source);
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
                        entryFname = PTStream.ReadCString(source, 28) + ".gvr";
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
                // Some Billy Hatcher textures have an oddity where the texture length is 16 more than what it
                // actually should be. This seems to only effect the last texture of a GVM, and only some of them
                // are affected. In that case, we will "fix" the GVRs in question.
                bool needToFix = (index == Files.Length - 1 && Files[index].Offset + Files[index].Length > Files[index].Stream.Length);

                // If this archive does not contain any global indicies, then just return the data as is.
                if (!containsGlobalIndex && !needToFix)
                {
                    return base.GetFile(index);
                }
                // Make sure index is not out of bounds
                if (index < 0 || index > Files.Length)
                    throw new IndexOutOfRangeException();

                long oldPosition = Files[index].Stream.Position;

                MemoryStream data = new MemoryStream();

                // Write out the GBIX header, if this archive contains global indicies
                if (containsGlobalIndex)
                {
                    data.WriteByte((byte)'G');
                    data.WriteByte((byte)'B');
                    data.WriteByte((byte)'I');
                    data.WriteByte((byte)'X');
                    PTStream.WriteInt32(data, 8);

                    Files[index].Stream.Position = 0xC + (index * tableEntryLength) + globalIndexOffset;
                    PTStream.WriteInt32BE(data, PTStream.ReadInt32BE(Files[index].Stream));

                    data.Position += 4;
                }

                // Now copy over the file data
                Files[index].Stream.Position = Files[index].Offset;
                PTStream.CopyPartTo(Files[index].Stream, data, Files[index].Length);

                // Fix the texture lengths for the textures that need to be "fixed"
                if (needToFix)
                {
                    if (containsGlobalIndex)
                    {
                        data.Position = 0x14;
                    }
                    else
                    {
                        data.Position = 0x4;
                    }

                    uint actualLength = PTStream.ReadUInt32(data);
                    data.Position -= 4;
                    PTStream.WriteUInt32(data, actualLength - 16);
                }

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
                // Only GVR textures can be added to a GVM archive.
                // If this is not a GVR texture, throw an exception
                if (!(new GvrTexture()).Is(source, length, fname))
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
                destination.WriteByte((byte)'G');
                destination.WriteByte((byte)'V');
                destination.WriteByte((byte)'M');
                destination.WriteByte((byte)'H');

                // Offset of the first texture in the archive
                long entryOffset = PTMethods.RoundUp(28 + (files.Count * entryLength), 16);
                PTStream.WriteInt32(destination, (int)entryOffset - 8);

                // Write out the flags
                PTStream.WriteUInt16BE(destination, flags);

                // Write out the number of files
                PTStream.WriteUInt16BE(destination, (ushort)files.Count);

                // We're going to be using this a few times. Might as well do this here
                long oldPosition;

                // Now, let's add the files
                for (int i = 0; i < files.Count; i++)
                {
                    // We need to get some information about the texture.
                    // We already checked to make sure this texture is a GVR.
                    // No need to check it again.
                    oldPosition = files[i].Stream.Position;
                    VrSharp.GvrTexture.GvrTexture texture = new VrSharp.GvrTexture.GvrTexture(files[i].Stream, files[i].Length);
                    //VrSharp.GvrTexture.GvrTextureInfo textureInfo = (VrSharp.GvrTexture.GvrTextureInfo)texture.GetTextureInfo();
                    files[i].Stream.Position = oldPosition;

                    // Write out the file number
                    PTStream.WriteUInt16BE(destination, (ushort)i);

                    // Write the information for this entry in the header
                    if (settings.Filename)
                    {
                        PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(files[i].Filename), 28);
                    }
                    if (settings.Formats)
                    {
                        destination.WriteByte((byte)(((byte)texture.PixelFormat << 4) | ((byte)texture.DataFlags & 0xF)));
                        destination.WriteByte((byte)texture.DataFormat);
                    }
                    if (settings.Dimensions)
                    {
                        ushort dimensions = 0;
                        dimensions |= (ushort)(((byte)Math.Log(texture.TextureWidth, 2) - 2) & 0xF);
                        dimensions |= (ushort)((((byte)Math.Log(texture.TextureHeight, 2) - 2) & 0xF) << 4);
                        PTStream.WriteUInt16BE(destination, dimensions);
                    }
                    if (settings.GlobalIndex)
                    {
                        PTStream.WriteUInt32BE(destination, texture.GlobalIndex);
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
            private PvmWriterSettings writerSettingsPanel;

            public bool Filename = true;
            public bool GlobalIndex = true;
            public bool Formats = true;
            public bool Dimensions = true;

            public override void SetPanelContent(Panel panel)
            {
                writerSettingsPanel = new PvmWriterSettings();
                panel.Controls.Add(writerSettingsPanel);
            }

            public override void SetSettings()
            {
                Filename = writerSettingsPanel.FilenameCheckbox.Checked;
                GlobalIndex = writerSettingsPanel.GlobalIndexCheckbox.Checked;
                Formats = writerSettingsPanel.FormatCheckbox.Checked;
                Dimensions = writerSettingsPanel.DimensionsCheckbox.Checked;
            }
        }
    }
}