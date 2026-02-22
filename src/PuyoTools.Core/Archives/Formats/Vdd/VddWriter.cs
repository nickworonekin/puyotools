using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PuyoTools.Core;

namespace PuyoTools.Archives.Formats.Vdd
{
    public class VddWriter : ArchiveWriter<ArchiveWriterEntry>
    {
        public VddWriter(Stream destination) : base(destination)
        {
        }

        public override void Write(Stream destination)
        {
            long streamStart = destination.Position;

            using BinaryWriter writer = new(destination, Encoding.UTF8, true);

            // Write the number of entries.
            writer.WriteInt32(_entries.Count);

            // Write the file entry table.
            long entryOffset = long.RoundUp(4 + (_entries.Count * 24), 2048);
            foreach (ArchiveWriterEntry entry in _entries)
            {
                writer.WriteString(entry.Name, 16);
                writer.WriteInt32((int)(entryOffset / 2048));
                writer.WriteInt32((int)entry.Length);

                entryOffset += long.RoundUp(entry.Length, 2048);
            }

            writer.Align(2048, streamStart);

            // Write the entry data.
            foreach (ArchiveWriterEntry entry in _entries)
            {
                WriteEntry(destination, entry);
                writer.Align(2048, streamStart);
            }
        }

        protected override ArchiveWriterEntry CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            return new ArchiveWriterEntry(source, name, leaveOpen);
        }
    }
}
