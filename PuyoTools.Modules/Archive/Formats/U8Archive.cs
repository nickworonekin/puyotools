using System;
using System.IO;

namespace PuyoTools.Modules.Archive
{
    public class U8Archive : ArchiveBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "U8"; }
        }

        /// <summary>
        /// The primary file extension for this archive format.
        /// </summary>
        public override string FileExtension
        {
            get { return ".arc"; }
        }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        public override ArchiveReader Open(Stream source)
        {
            return new U8ArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new U8ArchiveWriter(destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 32 && PTStream.Contains(source, 0, new byte[] { (byte)'U', 0xAA, (byte)'8', (byte)'-' }));
        }
    }

    #region Archive Reader
    public class U8ArchiveReader : ArchiveReader
    {
        public U8ArchiveReader(Stream source) : base(source)
        {
            // Read the archive header
            source.Position += 4;
            uint rootNodeOffset = PTStream.ReadUInt32BE(source);
            uint nodesLength = PTStream.ReadUInt32BE(source);
            uint dataOffset = PTStream.ReadUInt32BE(source);

            // Go the root node
            source.Position = startOffset + rootNodeOffset;
            Node rootNode = new Node();
            rootNode.Type = PTStream.ReadByte(source);
            rootNode.NameOffset = (uint)(PTStream.ReadByte(source) << 24 | PTStream.ReadUInt16BE(source));
            rootNode.DataOffset = PTStream.ReadUInt32BE(source);
            rootNode.Length = PTStream.ReadUInt32BE(source);

            entries = new ArchiveEntryCollection(this, (int)rootNode.Length - 1);
            uint stringTableOffset = rootNodeOffset + (rootNode.Length * 12);

            // rootNode.Length is essentially how many files are contained in the archive, so we'll do just that
            for (int i = 0; i < rootNode.Length - 1; i++)
            {
                // Read in this node
                Node node = new Node();
                node.Type = PTStream.ReadByte(source);
                node.NameOffset = (uint)(PTStream.ReadByte(source) << 24 | PTStream.ReadUInt16BE(source));
                node.DataOffset = PTStream.ReadUInt32BE(source);
                node.Length = PTStream.ReadUInt32BE(source);

                // Create the archive entry, then check what type of node it is
                long entryOffset = 0;
                int entryLength = 0;
                string entryFilename;

                // A file node
                if (node.Type == 0)
                {
                    entryOffset = startOffset + node.DataOffset;
                    entryLength = (int)node.Length;
                }

                // A directory node
                // In its present state, Puyo Tools can't handle directories in archives.
                // In the meantime, we'll just skip it
                else if (node.Type == 1)
                {
                    continue;
                }

                // Get the filename for the entry
                long oldPosition = source.Position;
                source.Position = startOffset + stringTableOffset + node.NameOffset;
                entryFilename = PTStream.ReadCString(source);
                source.Position = oldPosition;

                // Add this entry to the collection
                entries.Add(startOffset + entryOffset, entryLength, entryFilename);
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }

        private struct Node
        {
            public byte Type;
            public uint NameOffset;
            public uint DataOffset;
            public uint Length;
        }
    }
    #endregion

    #region Archive Writer
    public class U8ArchiveWriter : ArchiveWriter
    {
        public U8ArchiveWriter(Stream destination) : base(destination) { }

        public override void Flush()
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
                PTStream.CopyToPadded(entries[i].Open(), destination, 32, 0);

                // Call the file added event
                OnFileAdded(EventArgs.Empty);
            }
        }
    }
    #endregion
}