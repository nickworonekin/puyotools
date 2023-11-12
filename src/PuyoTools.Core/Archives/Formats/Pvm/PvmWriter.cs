using PuyoTools.Core;
using PuyoTools.Core.Archives;
using PuyoTools.Core.Textures.Pvr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Pvm
{
    public class PvmWriter : ArchiveWriter<PvmWriterEntry>
    {
        public PvmWriter(Stream destination) : base(destination)
        {
        }

        /// <summary>
        /// Gets or sets whether filenames should be stored for the entries. The default value is <see langword="true"/>.
        /// </summary>
        public bool HasFilenames { get; set; } = true;

        /// <summary>
        /// Gets or sets whether global indexes should be stored for the entries. The default value is <see langword="true"/>.
        /// </summary>
        public bool HasGlobalIndexes { get; set; } = true;

        /// <summary>
        /// Gets or sets whether palette and pixel formats should be stored within the file entry table. The default value is <see langword="true"/>.
        /// </summary>
        public bool HasFormats { get; set; } = true;

        /// <summary>
        /// Gets or sets whether texture dimensions should be stored within the file entry table. The default value is <see langword="true"/>.
        /// </summary>
        public bool HasDimensions { get; set; } = true;

        public override void Write(Stream destination)
        {
            // Calculate the length of each entry within the file entry table.
            // Additionally, set the flags that indicate what is stored within the file entry table.
            int entryLength = 2;
            PvmFlags flags = PvmFlags.PvrtChunks;

            if (HasFilenames)
            {
                entryLength += 28;
                flags |= PvmFlags.Filenames;
            }
            if (HasFormats)
            {
                entryLength += 2;
                flags |= PvmFlags.Formats;
            }
            if (HasDimensions)
            {
                entryLength += 2;
                flags |= PvmFlags.Dimensions;
            }
            if (HasGlobalIndexes)
            {
                entryLength += 4;
                flags |= PvmFlags.GlobalIndexes;
            }

            // Calculate the offset of the first entry.
            int firstEntryOffset = MathHelper.RoundUp(12 + (_entries.Count * entryLength), 16);

            long streamStart = destination.Position;

            using BinaryWriter writer = new(destination, Encoding.UTF8, true);

            // Write the header.
            writer.Write(PvmConstants.MagicCode);
            writer.WriteInt32(firstEntryOffset - 8);
            writer.WriteUInt16((ushort)flags);
            writer.WriteUInt16((ushort)_entries.Count);

            // Write the file entry table.
            for (int i = 0; i < _entries.Count; i++)
            {
                PvmWriterEntry entry = _entries[i];

                writer.WriteUInt16((ushort)i);

                if (HasFilenames)
                {
                    writer.WriteString(Path.GetFileNameWithoutExtension(entry.Name), 28);
                }
                if (HasFormats)
                {
                    writer.WriteByte(entry.PixelFormat);
                    writer.WriteByte(entry.DataFormat);
                }
                if (HasDimensions)
                {
                    ushort dimensions = 0;
                    dimensions |= (byte)((BitOperations.Log2(entry.Width) - 2) & 0xF);
                    dimensions |= (byte)(((BitOperations.Log2(entry.Width) - 2) & 0xF) << 4);
                    writer.WriteUInt16(dimensions);

                }
                if (HasGlobalIndexes)
                {
                    writer.WriteUInt32(entry.GlobalIndex ?? 0);
                }
            }

            writer.Align(16, streamStart);

            // Write the entry data.
            foreach (PvmWriterEntry entry in _entries)
            {
                WriteEntry(destination, entry);
                writer.Align(16, streamStart);
            }
        }

        protected override PvmWriterEntry? CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            // Only PVR textures can be added to this archive.
            if (!PvrTextureDecoder.Is(source))
            {
                return null;
            }

            return new PvmWriterEntry(source, name, leaveOpen);
        }
    }
}
