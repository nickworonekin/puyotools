using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools.Modules.Archive
{
    public class U8Archive : ArchiveBase
    {
        public override string Name
        {
            get { return "U8"; }
        }

        public override string FileExtension
        {
            get { return ".arc"; }
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
            return new Writer(destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 32 && PTStream.Contains(source, 0, new byte[] { (byte)'U', 0xAA, (byte)'8', (byte)'-' }));
        }

        public class Reader : ArchiveReader
        {
            public Reader(Stream source, int length)
            {
                // The start of the archive
                archiveOffset = source.Position;

                // Read the archive header
                source.Position += 4;
                uint rootNodeOffset = PTStream.ReadUInt32BE(source);
                uint nodesLength = PTStream.ReadUInt32BE(source);
                uint dataOffset = PTStream.ReadUInt32BE(source);

                // Go the root node
                source.Position = archiveOffset + rootNodeOffset;
                Node rootNode = new Node();
                rootNode.Type = PTStream.ReadByte(source);
                rootNode.NameOffset = (uint)(PTStream.ReadByte(source) << 24 | PTStream.ReadUInt16BE(source));
                rootNode.DataOffset = PTStream.ReadUInt32BE(source);
                rootNode.Length = PTStream.ReadUInt32BE(source);

                Files = new ArchiveEntry[rootNode.Length - 1];
                uint stringTableOffset = rootNodeOffset + (rootNode.Length * 12);

                // rootNode.Length is essentially how many files are contained in the archive, so we'll
                // do just that
                int shift = 0;
                for (int i = 0; i < rootNode.Length - 1; i++)
                {
                    // Read in this node
                    Node node = new Node();
                    node.Type = PTStream.ReadByte(source);
                    node.NameOffset = (uint)(PTStream.ReadByte(source) << 24 | PTStream.ReadUInt16BE(source));
                    node.DataOffset = PTStream.ReadUInt32BE(source);
                    node.Length = PTStream.ReadUInt32BE(source);

                    // Create the archive entry, then check what type of node it is
                    ArchiveEntry entry = new ArchiveEntry();
                    entry.Stream = source;

                    // A file node
                    if (node.Type == 0)
                    {
                        entry.Offset = archiveOffset + node.DataOffset;
                        entry.Length = (int)node.Length;
                    }

                    // A directory node
                    // In its present state, Puyo Tools can't handle directories in archives.
                    // In the meantime, we'll just skip it
                    else if (node.Type == 1)
                    {
                        shift++;
                        continue;
                    }

                    // Get the filename for the entry
                    long oldPosition = source.Position;
                    source.Position = archiveOffset + stringTableOffset + node.NameOffset;
                    entry.Filename = PTStream.ReadCString(source);
                    source.Position = oldPosition;

                    // Add this entry to the file list
                    Files[i - shift] = entry;
                }

                // Resize the array if there were directory entries
                if (shift != 0)
                {
                    Array.Resize<ArchiveEntry>(ref Files, Files.Length - shift);
                }

                // Set the position of the stream to the end of the file
                source.Position = archiveOffset + length;
            }

            private struct Node
            {
                public byte Type;
                public uint NameOffset;
                public uint DataOffset;
                public uint Length;
            }
        }

        public class Writer : ArchiveWriter
        {
            public Writer(Stream destination)
            {
                Initalize(destination);
            }

            public Writer(Stream destination, ArchiveWriterSettings settings) : this(destination) { }

            public override void Flush()
            {
                // The start of the archive
                long offset = destination.Position;

                // Puyo Tools is only capable of building U8 archives that do not contain directories.
                // It's just very difficult to do with the way Puyo Tools is structured.

                // First things first, let's get the header size
                int headerSize = ((files.Count + 1) * 12) + 1;
                for (int i = 0; i < files.Count; i++)
                {
                    headerSize += files[i].Filename.Length + 1;
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
                PTStream.WriteInt32BE(destination, files.Count + 1);

                nameOffset++;

                // Write out the file nodes
                for (int i = 0; i < files.Count; i++)
                {
                    destination.WriteByte(0);
                    destination.WriteByte((byte)(nameOffset >> 16));
                    PTStream.WriteUInt16BE(destination, (ushort)(nameOffset & 0xFFFF));
                    PTStream.WriteInt32BE(destination, dataOffset);
                    PTStream.WriteInt32BE(destination, files[i].Length);

                    nameOffset += files[i].Filename.Length + 1;
                    dataOffset += PTMethods.RoundUp(files[i].Length, 32);
                }

                // Write out the filename table
                PTStream.WriteCString(destination, String.Empty, 1);
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.WriteCString(destination, files[i].Filename, files[i].Filename.Length + 1);
                }

                // Pad
                while ((destination.Position - offset) % 32 != 0)
                    destination.WriteByte(0);

                // Write the file data
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.CopyPartToPadded(files[i].Stream, destination, files[i].Length, 32, 0);

                    // Call the file added event
                    OnFileAdded(EventArgs.Empty);
                }
            }
        }
    }
}