using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncodingExtensions = PuyoTools.Core.EncodingExtensions;

namespace PuyoTools.Archives.Formats.Mrg
{
    public class MrgWriter : ArchiveWriter<ArchiveWriterEntry>
    {
        public MrgWriter(Stream destination) : base(destination)
        {
        }

        public override void Write(Stream destination)
        {
            long streamStart = _destination.Position;

            using BinaryWriter writer = new(_destination, Encoding.UTF8, true);

            // Write the header and the number of entries.
            writer.Write(MrgConstants.MagicCode);
            writer.WriteInt32(_entries.Count);

            _destination.Position += 8;

            // Write the file entry table.
            int entryOffset = 16 + (_entries.Count * 48);
            foreach (ArchiveWriterEntry entry in _entries)
            {
                // Write the file extension, without the leading dot.
                string extension = Path.GetExtension(entry.Name);
                if (!string.IsNullOrEmpty(extension))
                {
                    extension = extension.Substring(1);
                }

                writer.WriteString(extension, 4, EncodingExtensions.ShiftJIS);

                // Write the offset, length, and filename (without the extension).
                writer.WriteInt32(entryOffset);
                writer.WriteInt32((int)entry.Length);

                _destination.Position += 4;

                writer.WriteString(Path.GetFileNameWithoutExtension(entry.Name), 32, EncodingExtensions.ShiftJIS);

                entryOffset += MathHelper.RoundUp((int)entry.Length, 16);
            }

            // Write the entry data.
            foreach (ArchiveWriterEntry entry in _entries)
            {
                WriteEntry(destination, entry);
                writer.Align(16, streamStart);
            }
        }

        protected override ArchiveWriterEntry? CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            throw new NotImplementedException();
        }
    }
}
