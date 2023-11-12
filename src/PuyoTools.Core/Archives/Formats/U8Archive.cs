using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class U8Archive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'U', 0xAA, (byte)'8', (byte)'-' };

        public override LegacyArchiveReader Open(Stream source)
        {
            return new U8ArchiveReader(source);
        }

        public override LegacyArchiveWriter Create(Stream destination)
        {
            return new U8ArchiveWriter(destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return source.Length - startPosition > 32
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode);
            }
        }
    }

    #region Archive Reader
    public class U8ArchiveReader : LegacyArchiveReader
    {
        public U8ArchiveReader(Stream source) : base(source)
        {
            using (var reader = source.AsBinaryReader())
            {
                // Read the archive header
                source.Position += 4;
                uint rootNodeOffset = reader.ReadUInt32BigEndian();
                uint nodesLength = reader.ReadUInt32BigEndian();
                uint dataOffset = reader.ReadUInt32BigEndian();

                // Only information that needs to be read from the root node is the last file index, which is the number of nodes the archive contains
                // The number of files will likely be less than the number of nodes, which is ok.
                source.Position = startOffset + rootNodeOffset + 8;

                var numberOfNodes = reader.ReadUInt32BigEndian();

                entries = new List<ArchiveEntry>((int)numberOfNodes);
                uint stringTableOffset = rootNodeOffset + (numberOfNodes * 12);

                // Create the root node, add it to the list of directory nodes, then set the current directory node to it.
                var rootNode = new DirectoryEntry
                {
                    Name = string.Empty,
                    Parent = null,
                    LastNodeIndex = numberOfNodes,
                };
                var directoryNodes = new List<DirectoryEntry>
                {
                    rootNode,
                };
                var currentDirectoryNode = rootNode;

                // Loop through the nodes
                // Since we already read the root node, we will start the loop at 1
                for (var i = 1; i < numberOfNodes; i++)
                {
                    // If we've reached the last node for this directory, go to the previous one in the list
                    if (i == currentDirectoryNode.LastNodeIndex)
                    {
                        directoryNodes.Remove(currentDirectoryNode);
                        currentDirectoryNode = directoryNodes.Last();
                    }

                    var nodeTypeAndNameOffset = reader.ReadUInt32BigEndian();
                    var nodeType = nodeTypeAndNameOffset >> 24;
                    var nameOffset = nodeTypeAndNameOffset & 0xFFFFFF;

                    if (nodeTypeAndNameOffset >> 24 == 1)
                    {
                        // Directory node
                        var directoryNode = new DirectoryEntry();
                        directoryNode.Name = reader.At(startOffset + stringTableOffset + nameOffset, x => x.ReadNullTerminatedString());
                        directoryNode.Parent = directoryNodes[reader.ReadInt32BigEndian()];
                        directoryNode.LastNodeIndex = reader.ReadUInt32BigEndian();

                        directoryNodes.Add(directoryNode);
                        currentDirectoryNode = directoryNode;
                    }
                    else
                    {
                        // File node
                        var entryName = $"{currentDirectoryNode.FullName}/{reader.At(startOffset + stringTableOffset + nameOffset, x => x.ReadNullTerminatedString())}";
                        var entryOffset = reader.ReadInt32BigEndian();
                        var entryLength = reader.ReadInt32BigEndian();

                        // Add this entry to the collection
                        entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, entryName));
                    }
                }

                // Set the position of the stream to the end of the file
                source.Seek(0, SeekOrigin.End);
            }
        }

        private class DirectoryEntry
        {
            public DirectoryEntry Parent;
            public string Name;
            public string FullName => Parent?.Parent != null
                ? $"{Parent.FullName}/{Name}"
                : Name;
            public uint LastNodeIndex;
        }
    }
    #endregion

    #region Archive Writer
    public class U8ArchiveWriter : LegacyArchiveWriter
    {
        public U8ArchiveWriter(Stream destination) : base(destination) { }

        protected override void WriteFile()
        {
            // The start of the archive
            long offset = destination.Position;

            // Puyo Tools is only capable of building U8 archives that do not contain directories.
            // It's just very difficult to do with the way Puyo Tools is structured.

            // First things first, let's get the header size
            int headerSize = ((entries.Count + 1) * 12) + 1;
            for (int i = 0; i < entries.Count; i++)
            {
                headerSize += entries[i].Name.Length + 1;
            }

            // Get the name and data offset
            int nameOffset = 0;
            int dataOffset = PTMethods.RoundUp(0x20 + headerSize, 32);

            // Start writing out the header
            destination.WriteByte((byte)'U');
            destination.WriteByte(0xAA);
            destination.WriteByte((byte)'8');
            destination.WriteByte((byte)'-');

            PTStream.WriteUInt32BE(destination, 0x20); // Root node offset (always 0x20)
            PTStream.WriteInt32BE(destination, headerSize); // Header size
            PTStream.WriteInt32BE(destination, dataOffset); // Data offset

            // Pad
            while ((destination.Position - offset) % 32 != 0)
                destination.WriteByte(0);

            // Write the root node
            destination.WriteByte(1);
            destination.WriteByte((byte)(nameOffset >> 16));
            PTStream.WriteUInt16BE(destination, (ushort)(nameOffset & 0xFFFF));
            PTStream.WriteInt32BE(destination, 0);
            PTStream.WriteInt32BE(destination, entries.Count + 1);

            nameOffset++;

            // Write out the file nodes
            for (int i = 0; i < entries.Count; i++)
            {
                destination.WriteByte(0);
                destination.WriteByte((byte)(nameOffset >> 16));
                PTStream.WriteUInt16BE(destination, (ushort)(nameOffset & 0xFFFF));
                PTStream.WriteInt32BE(destination, dataOffset);
                PTStream.WriteInt32BE(destination, entries[i].Length);

                nameOffset += entries[i].Name.Length + 1;
                dataOffset += PTMethods.RoundUp(entries[i].Length, 32);
            }

            // Write out the filename table
            PTStream.WriteCString(destination, String.Empty, 1);
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteCString(destination, entries[i].Name, entries[i].Name.Length + 1);
            }

            // Pad
            while ((destination.Position - offset) % 32 != 0)
                destination.WriteByte(0);

            // Write the file data
            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                PTStream.CopyToPadded(entries[i].Open(), destination, 32, 0);

                // Call the entry written event
                OnEntryWritten(new ArchiveEntryWrittenEventArgs(entries[i]));
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
                        currentDirectory.DirectoryEntries.Add(directory);
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
                currentDirectory.FileEntries.Add(fileEntry);
            }

            return rootDirectory;
        }

        private class FileEntry
        {
            public ArchiveEntry Entry;
            public string Name;
            public int Offset;
            public int Length;
        }

        private class DirectoryEntry
        {
            public DirectoryEntry Parent;
            public string Name;
            public uint LastNodeIndex;
            public readonly List<DirectoryEntry> DirectoryEntries = new List<DirectoryEntry>();
            public readonly List<FileEntry> FileEntries = new List<FileEntry>();
            public readonly Dictionary<string, DirectoryEntry> Directories = new Dictionary<string, DirectoryEntry>(StringComparer.OrdinalIgnoreCase);
        }
    }
    #endregion
}