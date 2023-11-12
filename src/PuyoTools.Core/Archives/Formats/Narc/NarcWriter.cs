using PuyoTools.Core.Archives;
using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    /// <summary>
    /// Archive writer for NARC archives.
    /// </summary>
    public partial class NarcWriter : ArchiveWriter<ArchiveWriterEntry>
    {
        public NarcWriter(Stream destination) : base(destination)
        {
        }

        /// <summary>
        /// Gets or sets whether filenames should be stored for the entries. The default value is <see langword="true"/>.
        /// </summary>
        public bool HasFilenames { get; set; } = true;

        public override void Write(Stream destination)
        {
            Hierarchy hierarchy = new(_entries);
            List<DirectoryEntry> directories = hierarchy.Directories;
            List<FileEntry> files = hierarchy.Files;
            int fimgLength = hierarchy.FileEntryDataLength;

            long streamStart = destination.Position;

            using BinaryWriter writer = new(destination, Encoding.UTF8, true);

            // Write the NARC header.
            writer.Write(NarcConstants.MagicCode);

            writer.WriteInt32(0); // File length (will be written to later)

            writer.WriteInt16(16); // Header length (always 16)
            writer.WriteInt16(3); // Number of sections (always 3)

            // Write the FATB section.
            writer.Write(NarcConstants.FatbMagicCode);

            writer.WriteInt32(12 + (files.Count * 8)); // Section length
            writer.WriteInt32(files.Count); // Number of file entries

            foreach (FileEntry file in files)
            {
                writer.WriteInt32(file.Offset); // Start position
                writer.WriteInt32(file.Offset + file.Length); // End position
            }

            // Write the FNTB section.
            writer.Write(NarcConstants.FntbMagicCode);

            if (HasFilenames)
            {
                int fntbPosition = (int)(_destination.Position - streamStart) - 4;

                writer.WriteInt32(0); // Section length (will be written to later)

                writer.WriteInt32(0); // Name entry offset for the root directory (will be written to later)
                writer.WriteInt16(0); // First file index (always 0)
                writer.WriteInt16((short)directories.Count); // Number of directories, including the root directory

                for (int i = 1; i < directories.Count; i++)
                {
                    writer.WriteInt32(0); // Name entry offset for this directory (will be written to later)
                    writer.WriteUInt16(directories[i].FirstFileIndex); // Index of the first file in this directory
                    writer.WriteUInt16((ushort)(directories[i].Parent!.Index | 0xF000)); // Parent directory
                }

                int nameEntryOffset = directories.Count * 8;
                foreach (DirectoryEntry directory in directories)
                {
                    directory.NameEntryOffset = nameEntryOffset;

                    foreach (Entry entry in directory.Entries)
                    {
                        byte[] nameAsBytes = Encoding.UTF8.GetBytes(entry.Name);

                        if (entry is DirectoryEntry directoryEntry)
                        {
                            writer.WriteByte((byte)(nameAsBytes.Length | 0x80)); // Length of the directory name
                            writer.Write(nameAsBytes);
                            writer.WriteUInt16((ushort)(directoryEntry.Index | 0xF000));

                            nameEntryOffset += nameAsBytes.Length + 3;
                        }
                        else if (entry is FileEntry fileEntry)
                        {
                            writer.WriteByte((byte)nameAsBytes.Length); // Length of the file name
                            writer.Write(nameAsBytes);

                            nameEntryOffset += nameAsBytes.Length + 1;
                        }
                    }

                    writer.WriteByte(0);

                    nameEntryOffset++;
                }

                writer.Align(4, streamStart, 0xFF);

                int fntbLength = (int)(_destination.Position - streamStart) - fntbPosition;

                // Go back and write the name entry offsets for each directory
                _destination.Position = streamStart + fntbPosition + 4;
                writer.Write(fntbLength);
                foreach (DirectoryEntry directory in directories)
                {
                    writer.Write(directory.NameEntryOffset);
                    _destination.Position += 4;
                }
                _destination.Position = streamStart + fntbPosition + fntbLength;
            }
            else
            {
                // The FNTB section is always the same if there are no filenames
                writer.WriteInt32(16); // Section length (always 16)
                writer.WriteInt32(4); // Always 4
                writer.WriteInt16(0); // First file index (always 0)
                writer.WriteInt16(1); // Number of directories, including the root directory (always 1)
            }

            // Write out the FIMG section
            writer.Write(NarcConstants.FimgMagicCode);

            writer.WriteInt32(fimgLength + 8); // Section length

            // Write the entry data.
            foreach (FileEntry file in files)
            {
                WriteEntry(destination, file.Entry);
                writer.Align(4, streamStart, 0xFF);
            }

            // Go back and write out the file length
            long endPosition = _destination.Position;
            writer.At(streamStart + 8, x => x.WriteInt32((int)(endPosition - streamStart))); // File length
        }

        protected override ArchiveWriterEntry CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            return new ArchiveWriterEntry(source, name, leaveOpen);
        }
    }
}
