using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Modules.Archive
{
    public class NarcArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'N', (byte)'A', (byte)'R', (byte)'C', 0xFE, 0xFF, 0x00, 0x01 };

        public override ArchiveReader Open(Stream source)
        {
            return new NarcArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new NarcArchiveWriter(destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;
            var remainingLength = source.Length - startPosition;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return remainingLength > 12
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)
                    && reader.At(startPosition + 8, x => x.ReadInt32()) == remainingLength;
            }
        }
    }

    #region Archive Reader
    public class NarcArchiveReader : ArchiveReader
    {
        public NarcArchiveReader(Stream source) : base(source)
        {
            using (var reader = source.AsBinaryReader())
            {
                // Read the archive header
                source.Position += 12;

                var headerLength = reader.ReadInt16();
                var fatbPosition = headerLength;

                // Read the FATB section
                source.Position = startOffset + fatbPosition + 4;

                var fatbLength = reader.ReadInt32();
                var fntbPosition = fatbPosition + fatbLength;

                // Get the number of entries in the archive
                var numberOfEntries = reader.ReadInt32();
                entries = new List<ArchiveEntry>(numberOfEntries);
                var fileEntries = new List<FileEntry>(numberOfEntries);
                for (var i = 0; i < numberOfEntries; i++)
                {
                    var offset = reader.ReadInt32();
                    var length = reader.ReadInt32() - offset;
                    fileEntries.Add(new FileEntry
                    {
                        Offset = offset,
                        Length = length,
                        Name = string.Empty,
                    });
                }

                // Read the FNTB section
                source.Position = fntbPosition + 4;

                var fntbLength = reader.ReadInt32();
                var fimgPosition = fntbPosition + fntbLength;

                var hasFilenames = true;

                // If the FNTB length is 16 or less, it's impossible for the entries to have filenames.
                // This section will always be at least 16 bytes long, but technically it's only required to be at least 8 bytes long.
                if (fntbLength <= 16)
                {
                    hasFilenames = false;
                }

                var rootNameEntryOffset = reader.ReadInt32();

                // If the root name entry offset is 4, then the entries don't have filenames.
                if (rootNameEntryOffset == 4)
                {
                    hasFilenames = false;
                }

                if (hasFilenames)
                {
                    var rootFirstFileIndex = reader.ReadInt16();
                    var rootDirectory = new DirectoryEntry
                    {
                        Name = string.Empty,
                    };

                    var directoryEntryCount = reader.ReadInt16(); // This includes the root directory
                    var directoryEntries = new List<DirectoryEntry>(directoryEntryCount)
                    {
                        rootDirectory,
                    };

                    // This NARC contains filenames and directory names, so read them
                    for (var i = 1; i < directoryEntryCount; i++)
                    {
                        var nameEntryTableOffset = reader.ReadInt32();
                        var firstFileIndex = reader.ReadInt16();
                        var parentDirectoryIndex = reader.ReadInt16() & 0xFFF;

                        directoryEntries.Add(new DirectoryEntry
                        {
                            Parent = directoryEntries[parentDirectoryIndex],
                            NameEntryOffset = nameEntryTableOffset,
                            FirstFileIndex = firstFileIndex,
                        });
                    }

                    var currentDirectory = rootDirectory;
                    var directoryIndex = 0;
                    var fileIndex = 0;
                    while (directoryIndex < directoryEntryCount)
                    {
                        var entryNameLength = reader.ReadByte();
                        if ((entryNameLength & 0x80) != 0)
                        {
                            // This is a directory name entry
                            var entryName = reader.ReadString(entryNameLength & 0x7F);
                            var entryDirectoryIndex = reader.ReadInt16() & 0xFFF;
                            var directoryEntry = directoryEntries[entryDirectoryIndex];

                            directoryEntry.Name = entryName;
                        }
                        else if (entryNameLength != 0)
                        {
                            // This is a file name entry
                            var entryName = reader.ReadString(entryNameLength);
                            var fileEntry = fileEntries[fileIndex];

                            fileEntry.Parent = directoryEntries[directoryIndex];
                            fileEntry.Name = entryName;

                            fileIndex++;
                        }
                        else
                        {
                            // This is the end of a directory
                            directoryIndex++;
                            if (directoryIndex >= directoryEntryCount)
                            {
                                break;
                            }
                            currentDirectory = directoryEntries[directoryIndex];
                        }
                    }
                }

                // Now create the ArchiveEntry instances from the FileEntry instances
                foreach (var fileEntry in fileEntries)
                {
                    entries.Add(new ArchiveEntry(this, startOffset + fimgPosition + 8 + fileEntry.Offset, fileEntry.Length, fileEntry.FullName));
                }
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }

        private class FileEntry
        {
            public DirectoryEntry Parent;
            public int Offset;
            public int Length;
            public string Name;
            public string FullName => Parent?.Parent != null
                ? $"{Parent.FullName}/{Name}"
                : Name;
        }

        private class DirectoryEntry
        {
            public DirectoryEntry Parent;
            public int NameEntryOffset;
            public int FirstFileIndex;
            public string Name;
            public string FullName => Parent?.Parent != null
                ? $"{Parent.FullName}/{Name}"
                : Name;
        }
    }
    #endregion

    #region Archive Writer
    public class NarcArchiveWriter : ArchiveWriter
    {
        public NarcArchiveWriter(Stream destination) : base(destination) { }

        protected override void WriteFile()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}