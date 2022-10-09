using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class SvmArchiveEntry : ArchiveEntry
    {
        internal SvmArchiveEntry(LegacyArchiveReader archiveReader, long offset, int length, string name, uint? globalIndex)
            : base(archiveReader, offset, length, name)
        {
            GlobalIndex = globalIndex;
        }

        public uint? GlobalIndex { get; }

        /// <inheritdoc/>
        public override Stream Open()
        {
            // If the texture associated with this entry does not have a global index, just return the data as-is.
            if (!GlobalIndex.HasValue)
            {
                return base.Open();
            }

            // Add a GBIX header
            var data = new MemoryStream(Length + 16);
            using (var writer = data.AsBinaryWriter())
            {
                writer.Write(new byte[] { (byte)'G', (byte)'B', (byte)'I', (byte)'X' });
                writer.WriteInt32(8);
                writer.WriteUInt32(GlobalIndex.Value);
                writer.WriteInt32(0);
            }
            using (var source = new StreamView(archiveReader.ArchiveStream, offset, length))
            {
                source.CopyTo(data);
            }

            data.Seek(0, SeekOrigin.Begin);

            return data;
        }
    }
}
