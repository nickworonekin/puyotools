using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    /// <summary>
    /// Archive reader for NARC archives.
    /// </summary>
    public partial class NarcReader : ArchiveReader<ArchiveReaderEntry>
    {
        public NarcReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a NARC archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Read the archive header.
            _stream.Position += 12;

            short headerLength = reader.ReadInt16();
            short fatbPosition = headerLength;

            // Read the FATB section
            source.Position = _streamStart + fatbPosition + 4;

            int fatbLength = reader.ReadInt32();
            int fntbPosition = fatbPosition + fatbLength;

            // Get the number of entries in the archive.
            int numEntries = reader.ReadInt32();
            _entries = new List<ArchiveReaderEntry>(numEntries);
            List<FileEntry> fileEntries = new(numEntries);

            // Loop through all of the entries.
            for (var i = 0; i < numEntries; i++)
            {
                int offset = reader.ReadInt32();
                int length = reader.ReadInt32() - offset;

                fileEntries.Add(new FileEntry
                {
                    Offset = offset,
                    Length = length,
                    Name = string.Empty, // This may be filled in later.
                });
            }

            // Read the FNTB section.
            _stream.Position = fntbPosition + 4;

            int fntbLength = reader.ReadInt32();
            int fimgPosition = fntbPosition + fntbLength;

            bool hasFilenames = true;

            // If the FNTB length is 16 or less, it's impossible for the entries to have filenames.
            // This section will always be at least 16 bytes long, but technically it's only required to be at least 8 bytes long.
            if (fntbLength <= 16)
            {
                hasFilenames = false;
            }

            int rootNameEntryOffset = reader.ReadInt32();

            // If the root name entry offset is 4, then the entries don't have filenames.
            if (rootNameEntryOffset == 4)
            {
                hasFilenames = false;
            }

            if (hasFilenames)
            {
                short rootFirstFileIndex = reader.ReadInt16();
                DirectoryEntry rootDirectory = new()
                {
                    Name = string.Empty,
                };

                short directoryEntryCount = reader.ReadInt16(); // This includes the root directory.
                List<DirectoryEntry> directoryEntries = new(directoryEntryCount)
                {
                    rootDirectory,
                };

                // This NARC contains filenames and directory names, so read them.
                for (int i = 1; i < directoryEntryCount; i++)
                {
                    int nameEntryTableOffset = reader.ReadInt32();
                    short firstFileIndex = reader.ReadInt16();
                    short parentDirectoryIndex = (short)(reader.ReadInt16() & 0xFFF);

                    directoryEntries.Add(new DirectoryEntry
                    {
                        Parent = directoryEntries[parentDirectoryIndex],
                        NameEntryOffset = nameEntryTableOffset,
                        FirstFileIndex = firstFileIndex,
                    });
                }

                DirectoryEntry currentDirectory = rootDirectory;
                int directoryIndex = 0;
                int fileIndex = 0;
                while (directoryIndex < directoryEntryCount)
                {
                    byte entryNameLength = reader.ReadByte();
                    if ((entryNameLength & 0x80) != 0)
                    {
                        // This is a directory name entry
                        string entryName = reader.ReadString(entryNameLength & 0x7F);
                        short entryDirectoryIndex = (short)(reader.ReadInt16() & 0xFFF);
                        DirectoryEntry directoryEntry = directoryEntries[entryDirectoryIndex];

                        directoryEntry.Name = entryName;
                    }
                    else if (entryNameLength != 0)
                    {
                        // This is a file name entry.
                        string entryName = reader.ReadString(entryNameLength);
                        FileEntry fileEntry = fileEntries[fileIndex];

                        fileEntry.Parent = directoryEntries[directoryIndex];
                        fileEntry.Name = entryName;

                        fileIndex++;
                    }
                    else
                    {
                        // This is the end of a directory.
                        directoryIndex++;
                        if (directoryIndex >= directoryEntryCount)
                        {
                            break;
                        }
                        currentDirectory = directoryEntries[directoryIndex];
                    }
                }
            }

            // Add the FileEntry instances to the entry collection.
            foreach (FileEntry fileEntry in fileEntries)
            {
                _entries.Add(new ArchiveReaderEntry(_stream, _streamStart + fimgPosition + 8 + fileEntry.Offset, fileEntry.Length, fileEntry.FullName));
            }

            // We're done reading this archive. Seek to the end of the stream.
            source.Seek(0, SeekOrigin.End);
        }

        /// <summary>
        /// Returns if the data in <paramref name="source"/> resembles an archive of this format.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns><see langword="true"/> if the data resembles an archive of this format, <see langword="false"/> otherwise.</returns>
        public static bool IsFormat(Stream source)
        {
            long startPosition = source.Position;
            long remainingLength = source.Length - startPosition;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            return remainingLength > 12
                && reader.At(startPosition, x => x.ReadBytes(NarcConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(NarcConstants.MagicCode)
                && reader.At(startPosition + 8, x => x.ReadInt32()) == remainingLength;
        }
    }
}
