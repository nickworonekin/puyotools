using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class NarcArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'N', (byte)'A', (byte)'R', (byte)'C', 0xFE, 0xFF, 0x00, 0x01 };

        public override LegacyArchiveReader Open(Stream source)
        {
            return new NarcArchiveReader(source);
        }

        public override LegacyArchiveWriter Create(Stream destination)
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
    public class NarcArchiveReader : LegacyArchiveReader
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
                        Name = string.Empty, // This may be filled in later
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
    public class NarcArchiveWriter : LegacyArchiveWriter
    {
        public bool HasFilenames { get; set; } = true;

        public NarcArchiveWriter(Stream destination) : base(destination) { }

        protected override void WriteFile()
        {
            var startPosition = destination.Position;

            var rootDirectory = CreateHeiarchy();

            var directories = new List<DirectoryEntry>
            {
                rootDirectory,
            };
            var files = new List<FileEntry>();

            ushort directoryIndex = 1; // The root directory index is always 0.
            ushort fileIndex = 0;
            var position = 0;
            for (var i = 0; i < directories.Count; i++) // We'll be modifying directories during the loop, so we can't use a foreach loop here.
            {
                foreach (var entry in directories[i].Entries)
                {
                    if (entry is DirectoryEntry directoryEntry)
                    {
                        directoryEntry.Index = directoryIndex;
                        directoryEntry.FirstFileIndex = fileIndex;

                        directories.Add(directoryEntry);

                        directoryIndex++;
                    }
                    else if (entry is FileEntry fileEntry)
                    {
                        fileEntry.Index = fileIndex;
                        fileEntry.Offset = position;

                        position += ((fileEntry.Length + 3) / 4) * 4; // Offsets must be a multiple of 4

                        files.Add(fileEntry);

                        fileIndex++;
                    }
                }
            }
            var fimgLength = position;

            using (var writer = new BinaryWriter(destination))
            {
                // Write out the NARC header
                writer.Write((byte)'N');
                writer.Write((byte)'A');
                writer.Write((byte)'R');
                writer.Write((byte)'C');
                writer.Write((byte)0xFE);
                writer.Write((byte)0xFF);
                writer.Write((byte)0);
                writer.Write((byte)1);

                writer.WriteInt32(0); // File length (will be written to later)

                writer.WriteInt16(16); // Header length (always 16)
                writer.WriteInt16(3); // Number of sections (always 3)

                // Write out the FATB section
                writer.Write((byte)'B');
                writer.Write((byte)'T');
                writer.Write((byte)'A');
                writer.Write((byte)'F');

                writer.WriteInt32(12 + (files.Count * 8)); // Section length
                writer.WriteInt32(files.Count); // Number of file entries

                foreach (var file in files)
                {
                    writer.WriteInt32(file.Offset); // Start position
                    writer.WriteInt32(file.Offset + file.Length); // End position
                }

                // Write out the FNTB section
                writer.Write((byte)'B');
                writer.Write((byte)'T');
                writer.Write((byte)'N');
                writer.Write((byte)'F');

                if (HasFilenames)
                {
                    var fntbPosition = (int)(destination.Position - startPosition) - 4;

                    writer.WriteInt32(0); // Section length (will be written to later)

                    writer.WriteInt32(0); // Name entry offset for the root directory (will be written to later)
                    writer.WriteInt16(0); // First file index (always 0)
                    writer.WriteInt16((short)directories.Count); // Number of directories, including the root directory

                    for (var i = 1; i < directories.Count; i++)
                    {
                        writer.WriteInt32(0); // Name entry offset for this directory (will be written to later)
                        writer.WriteUInt16(directories[i].FirstFileIndex); // Index of the first file in this directory
                        writer.WriteUInt16((ushort)(directories[i].Parent.Index | 0xF000)); // Parent directory
                    }

                    position = directories.Count * 8;
                    foreach (var directory in directories)
                    {
                        directory.NameEntryOffset = position;

                        foreach (var entry in directory.Entries)
                        {
                            var nameAsBytes = Encoding.UTF8.GetBytes(entry.Name);

                            if (entry is DirectoryEntry directoryEntry)
                            {
                                writer.WriteByte((byte)(nameAsBytes.Length | 0x80)); // Length of the directory name
                                writer.Write(nameAsBytes);
                                writer.WriteUInt16((ushort)(directoryEntry.Index | 0xF000));

                                position += nameAsBytes.Length + 3;
                            }
                            else if (entry is FileEntry fileEntry)
                            {
                                writer.WriteByte((byte)nameAsBytes.Length); // Length of the file name
                                writer.Write(nameAsBytes);

                                position += nameAsBytes.Length + 1;
                            }
                        }

                        writer.WriteByte(0);

                        position++;
                    }

                    while ((destination.Length - startPosition) % 4 != 0)
                    {
                        writer.WriteByte(0xFF);
                    }

                    var fntbLength = (int)(destination.Position - startPosition) - fntbPosition;

                    // Go back and write the name entry offsets for each directory
                    destination.Position = startPosition + fntbPosition + 4;
                    writer.Write(fntbLength);
                    foreach (var directory in directories)
                    {
                        writer.Write(directory.NameEntryOffset);
                        destination.Position += 4;
                    }
                    destination.Position = startPosition + fntbPosition + fntbLength;
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
                writer.Write((byte)'G');
                writer.Write((byte)'M');
                writer.Write((byte)'I');
                writer.Write((byte)'F');

                writer.WriteInt32(fimgLength + 8); // Section length

                foreach (var file in files)
                {
                    // Call the entry writing event
                    OnEntryWriting(new ArchiveEntryWritingEventArgs(file.Entry));

                    using (var input = file.Entry.Open())
                    {
                        input.CopyTo(destination);
                    }

                    while ((destination.Length - startPosition) % 4 != 0)
                    {
                        writer.WriteByte(0xFF);
                    }

                    // Call the entry written event
                    OnEntryWritten(new ArchiveEntryWrittenEventArgs(file.Entry));
                }

                // Go back and write out the file length
                var endPosition = destination.Position;
                writer.At(startPosition + 8, x => x.WriteInt32((int)(endPosition - startPosition))); // File length
            }
        }

        private DirectoryEntry CreateHeiarchy()
        {
            var rootDirectory = new DirectoryEntry
            {
                Name = string.Empty,
            };

            foreach (var entry in entries)
            {
                var currentDirectory = rootDirectory;

                var paths = entry.FullName.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                var pathIndex = 0;
                for (; pathIndex < paths.Length - 1; pathIndex++)
                {
                    if (!currentDirectory.Directories.TryGetValue(paths[pathIndex], out var directory))
                    {
                        directory = new DirectoryEntry
                        {
                            Parent = currentDirectory,
                            Name = paths[pathIndex],
                        };
                        currentDirectory.Entries.Add(directory);
                        currentDirectory.Directories.Add(directory.Name, directory);
                    }

                    currentDirectory = directory;
                }

                var fileEntry = new FileEntry
                {
                    Entry = entry,
                    Name = paths[pathIndex],
                    Length = entry.Length,
                };
                currentDirectory.Entries.Add(fileEntry);
            }

            return rootDirectory;
        }

        private abstract class Entry
        {
            public ushort Index;
            public string Name;
        }

        private class FileEntry : Entry
        {
            public ArchiveEntry Entry;
            public int Offset;
            public int Length;
        }

        private class DirectoryEntry : Entry
        {
            public DirectoryEntry Parent;
            public int NameEntryOffset;
            public ushort FirstFileIndex;
            public readonly List<Entry> Entries = new List<Entry>();
            public readonly Dictionary<string, DirectoryEntry> Directories = new Dictionary<string, DirectoryEntry>(StringComparer.OrdinalIgnoreCase);
        }
    }
    #endregion
}