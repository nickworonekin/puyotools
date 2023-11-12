using PuyoTools.Core.Archives;
using PuyoTools.Core;
using PuyoTools.Core.Textures.Gvr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace PuyoTools.Archives.Formats.Gvm
{
    public class GvmWriter : ArchiveWriter<GvmWriterEntry>
    {
        public GvmWriter(Stream destination) : base(destination)
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
            GvmFlags flags = 0;

            if (HasFilenames)
            {
                entryLength += 28;
                flags |= GvmFlags.Filenames;
            }
            if (HasFormats)
            {
                entryLength += 2;
                flags |= GvmFlags.Formats;
            }
            if (HasDimensions)
            {
                entryLength += 2;
                flags |= GvmFlags.Dimensions;
            }
            if (HasGlobalIndexes)
            {
                entryLength += 4;
                flags |= GvmFlags.GlobalIndexes;
            }

            // Calculate the offset of the first entry.
            int firstEntryOffset = MathHelper.RoundUp(28 + (_entries.Count * entryLength), 16);

            long streamStart = destination.Position;

            using BinaryWriter writer = new(destination, Encoding.UTF8, true);

            // Write the header.
            writer.Write(GvmConstants.MagicCode);
            writer.WriteInt32(firstEntryOffset - 8);
            writer.WriteUInt16BigEndian((ushort)flags);
            writer.WriteUInt16BigEndian((ushort)_entries.Count);

            // Write the file entry table.
            for (int i = 0; i < _entries.Count; i++)
            {
                GvmWriterEntry entry = _entries[i];

                writer.WriteUInt16BigEndian((ushort)i);

                if (HasFilenames)
                {
                    writer.WriteString(Path.GetFileNameWithoutExtension(entry.Name), 28);
                }
                if (HasFormats)
                {
                    writer.WriteByte(entry.FlagsAndPaletteFormat);
                    writer.WriteByte(entry.PixelFormat);
                }
                if (HasDimensions)
                {
                    ushort dimensions = 0;
                    dimensions |= (byte)((BitOperations.Log2(entry.Width) - 2) & 0xF);
                    dimensions |= (byte)(((BitOperations.Log2(entry.Width) - 2) & 0xF) << 4);
                    writer.WriteUInt16BigEndian(dimensions);
                }
                if (HasGlobalIndexes)
                {
                    writer.WriteUInt32BigEndian(entry.GlobalIndex ?? 0);
                }
            }

            writer.Write(new byte[16]);

            writer.Align(16, streamStart);

            // Write the entry data.
            foreach (GvmWriterEntry entry in _entries)
            {
                WriteEntry(destination, entry);
                writer.Align(16, streamStart);
            }
        }

        protected override GvmWriterEntry? CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            // Only GVR textures can be added to this archive.
            if (!GvrTextureDecoder.Is(source))
            {
                return null;
            }

            return new GvmWriterEntry(source, name, leaveOpen);
        }
    }
}
