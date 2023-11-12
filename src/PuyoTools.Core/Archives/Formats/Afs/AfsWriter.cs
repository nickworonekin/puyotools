using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Afs
{
    public class AfsWriter : ArchiveWriter<AfsWriterEntry>
    {
        private AfsVersion _version = AfsVersion.Version1;

        public AfsWriter(Stream destination) : base(destination)
        {
        }

        /// <summary>
        /// Gets or sets the block size.
        /// </summary>
        public int BlockSize { get; set; } = 2048/*4*/;

        /// <summary>
        /// Gets or sets the AFS version.
        /// </summary>
        public AfsVersion Version
        {
            get => _version;
            set
            {
                ArgumentHelper.ThrowIfInvalidEnumValue(value);

                _version = value;
            }
        }

        /// <summary>
        /// Gets or sets whether timestamps should be written to the entry metadata.
        /// </summary>
        public bool HasTimestamps { get; set; }

        public override void Write(Stream destination)
        {
            long streamStart = destination.Position;

            using BinaryReader reader = new(destination, Encoding.UTF8, true);
            using BinaryWriter writer = new(destination, Encoding.UTF8, true);

            // Write the header and the number of entries.
            writer.Write(AfsConstants.MagicCode);
            writer.WriteInt32(_entries.Count);

            // Write the file entry table.
            int firstEntryOffset = MathHelper.RoundUp(16 + (_entries.Count * 8), BlockSize);
            int entryOffset = firstEntryOffset;
            foreach (ArchiveWriterEntry entry in _entries)
            {
                writer.WriteInt32(entryOffset);
                writer.WriteInt32((int)entry.Length);

                entryOffset += MathHelper.RoundUp((int)entry.Length, BlockSize);
            }
            int metadataOffset = entryOffset;

            // If this is a V1 archive, write the metadata offset.
            // In V1 archives, it's stored after the last entry in the entry table.
            if (_version == AfsVersion.Version1)
            {
                writer.WriteInt32(metadataOffset);
                writer.WriteInt32(_entries.Count * 48);
            }

            writer.Align(BlockSize, streamStart);

            // If this is a V2 archive, write the metadata offset.
            // In V2 archives, it's stored before the data of the first entry.
            if (_version == AfsVersion.Version2)
            {
                writer.At(streamStart + firstEntryOffset - 8, x =>
                {
                    writer.WriteInt32(metadataOffset);
                    writer.WriteInt32(_entries.Count * 48);
                });
            }

            // Write the entry data.
            foreach (AfsWriterEntry entry in _entries)
            {
                WriteEntry(destination,entry);
                writer.Align(BlockSize, streamStart);
            }

            // Write the metadata.
            for (int i = 0; i < _entries.Count; i++)
            {
                AfsWriterEntry entry = _entries[i];

                // Write the filename.
                writer.WriteString(entry.Name, 32);

                // Write the entry's timestamp (we use the file's last modified time).
                // If this timestamp isn't present, or we don't want to write it, just write null bytes.
                if (HasTimestamps && entry.Timestamp.HasValue)
                {
                    DateTime timestamp = entry.Timestamp.Value;

                    writer.WriteUInt16((ushort)timestamp.Year);
                    writer.WriteUInt16((ushort)timestamp.Month);
                    writer.WriteUInt16((ushort)timestamp.Day);
                    writer.WriteUInt16((ushort)timestamp.Hour);
                    writer.WriteUInt16((ushort)timestamp.Minute);
                    writer.WriteUInt16((ushort)timestamp.Second);
                }
                else
                {
                    writer.Write(new byte[12]);
                }

                // The last 4 bytes of the entry's metadata appear to pull from the header.
                // For V1 archives, this corresponds to the entry's data offset.
                // For V2 archives, it just pulls 4 bytes in sequential order, starting with the bytes
                // corresponding to the number of entries in the archive.
                if (_version == AfsVersion.Version1)
                {
                    writer.Write(reader.At(streamStart + 8 + (i * 8), x => x.ReadBytes(4)));
                }
                else
                {
                    writer.Write(reader.At(streamStart + 4 + (i * 4), x => x.ReadBytes(4)));
                }
            }

            writer.Align(BlockSize, streamStart);
        }

        protected override AfsWriterEntry CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            return new AfsWriterEntry(source, name, leaveOpen);
        }
    }
}
