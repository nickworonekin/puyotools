using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.U8
{
    public partial class U8Reader : ArchiveReader<ArchiveReaderEntry>
    {
        /// <summary>
        /// Archive reader for U8 archives.
        /// </summary>
        public U8Reader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a U8 archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Read the archive header.
            _stream.Position += 4;
            uint rootNodeOffset = reader.ReadUInt32BigEndian();
            uint nodesLength = reader.ReadUInt32BigEndian();
            uint dataOffset = reader.ReadUInt32BigEndian();

            // Only information that needs to be read from the root node is the last file index, which is the number of nodes the archive contains.
            // The number of files will likely be less than the number of nodes, which is ok.
            _stream.Position = _streamStart + rootNodeOffset + 8;

            // Get the number of entries in the archive.
            // If there are directories in the archive, this number will be greater than the number of files.
            // We'll still set the initial capacity to the node count.
            uint numberOfNodes = reader.ReadUInt32BigEndian();

            _entries = new List<ArchiveReaderEntry>((int)numberOfNodes);
            uint stringTableOffset = rootNodeOffset + (numberOfNodes * 12);

            // Create the root node, add it to the list of directory nodes, then set the current directory node to it.
            DirectoryEntry rootNode = new()
            {
                Name = string.Empty,
                Parent = null,
                LastNodeIndex = numberOfNodes,
            };
            List<DirectoryEntry> directoryNodes = new()
            {
                rootNode,
            };
            DirectoryEntry currentDirectoryNode = rootNode;

            // Loop through all of the nodes.
            // Since we have already read the root node, we will start the loop index at 1.
            for (uint i = 1; i < numberOfNodes; i++)
            {
                // If we've reached the last node for this directory, go to the previous one in the list.
                if (i == currentDirectoryNode.LastNodeIndex)
                {
                    directoryNodes.Remove(currentDirectoryNode);
                    currentDirectoryNode = directoryNodes.Last();
                }

                uint nodeTypeAndNameOffset = reader.ReadUInt32BigEndian();
                uint nodeType = nodeTypeAndNameOffset >> 24;
                uint nameOffset = nodeTypeAndNameOffset & 0xFFFFFF;

                if (nodeType == 1)
                {
                    // Directory node
                    DirectoryEntry directoryNode = new();
                    directoryNode.Name = reader.At(_streamStart + stringTableOffset + nameOffset, x => x.ReadNullTerminatedString());
                    directoryNode.Parent = directoryNodes[reader.ReadInt32BigEndian()];
                    directoryNode.LastNodeIndex = reader.ReadUInt32BigEndian();

                    directoryNodes.Add(directoryNode);
                    currentDirectoryNode = directoryNode;
                }
                else
                {
                    // File node
                    string entryName = reader.At(_streamStart + stringTableOffset + nameOffset, x => x.ReadNullTerminatedString());
                    int entryOffset = reader.ReadInt32BigEndian();
                    int entryLength = reader.ReadInt32BigEndian();

                    // Prepend the directory path if the current directory is not the root directory.
                    if (currentDirectoryNode.Parent is not null)
                    {
                        entryName = $"{currentDirectoryNode.FullName}/{entryName}";
                    }

                    // Add this entry to the entry collection.
                    _entries.Add(new ArchiveReaderEntry(_stream, _streamStart + entryOffset, entryLength, entryName));
                }
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

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            return source.Length - startPosition > 32
                && reader.At(startPosition, x => x.ReadBytes(U8Constants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(U8Constants.MagicCode);
        }
    }
}
