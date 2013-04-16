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

        public override bool CanCreate
        {
            get { return false; }
        }

        public override ArchiveReader Open(Stream source, int length)
        {
            return new Reader(source, length);
        }

        public override ArchiveWriter Create(Stream destination, ArchiveWriterSettings settings)
        {
            return new Writer(destination);
        }

        public override ArchiveWriterSettings GetWriterSettings()
        {
            return null;
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
                rootNode.Type = PTStream.ReadUInt16BE(source);
                rootNode.NameOffset = PTStream.ReadUInt16BE(source);
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
                    node.Type = PTStream.ReadUInt16BE(source);
                    node.NameOffset = PTStream.ReadUInt16BE(source);
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
                public ushort Type;
                public ushort NameOffset;
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
                throw new NotImplementedException();
            }
        }
    }
}