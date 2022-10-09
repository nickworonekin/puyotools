using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Pvm
{
    public class PvmReaderEntry : ArchiveReaderEntry
    {
        public PvmReaderEntry(Stream stream, long position, long length, string? name, uint? globalIndex)
            : base(stream, position, length, name)
        {
            GlobalIndex = globalIndex;
        }

        /// <summary>
        /// Gets the global index for the entry's texture, or <see langword="null"/> if no global index exists.
        /// </summary>
        public uint? GlobalIndex { get; }

        public override Stream Open()
        {
            // If the texture associated with this entry does not have a global index, just return the data as-is.
            if (!GlobalIndex.HasValue)
            {
                return base.Open();
            }

            MemoryStream stream = new((int)(Length + 16));
            using BinaryWriter writer = new(stream, Encoding.UTF8, true);

            // Write the GBIX header.
            writer.Write(PvmConstants.GbixMagicCode);
            writer.WriteInt32(8);
            writer.WriteUInt32(GlobalIndex.Value);
            writer.WriteInt32(0);

            // Write the remaining texture data.
            base.Open().CopyTo(stream);

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
