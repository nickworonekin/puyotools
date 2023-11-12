using PuyoTools.Core.Archives;
using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Acx
{
    public class AcxWriter : ArchiveWriter<ArchiveWriterEntry>
    {
        public AcxWriter(Stream destination) : base(destination)
        {
        }

        /// <summary>
        /// Gets or sets the block size.
        /// </summary>
        public int BlockSize { get; set; } = 4;

        public override void Write(Stream destination)
        {
            long streamStart = destination.Position;

            using BinaryWriter writer = new(destination, Encoding.UTF8, true);

            // Write the header and the number of entries.
            writer.Write(AcxConstants.MagicCode);
            writer.WriteInt32BigEndian(_entries.Count);

            // Write the file entry table.
            int entryOffset = MathHelper.RoundUp(8 + (_entries.Count * 8), BlockSize);
            foreach (ArchiveWriterEntry entry in _entries)
            {
                writer.WriteInt32BigEndian(entryOffset);
                writer.WriteInt32BigEndian((int)entry.Length);

                entryOffset += MathHelper.RoundUp((int)entry.Length, BlockSize);
            }

            writer.Align(BlockSize, streamStart);

            // Write the entry data.
            foreach (ArchiveWriterEntry entry in _entries)
            {
                WriteEntry(destination, entry);
                writer.Align(BlockSize, streamStart);
            }
        }

        protected override ArchiveWriterEntry CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            return new ArchiveWriterEntry(source, null, leaveOpen);
        }
    }
}
