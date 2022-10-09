using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class GvmArchiveEntry : ArchiveEntry
    {
        private bool needToFix;

        internal GvmArchiveEntry(LegacyArchiveReader archiveReader, long offset, int length, string name, uint? globalIndex, bool needToFix)
            : base(archiveReader, offset, length, name)
        {
            GlobalIndex = globalIndex;
            this.needToFix = needToFix;
        }

        public uint? GlobalIndex { get; }

        /// <inheritdoc/>
        public override Stream Open()
        {
            // If the texture associated with this entry does not have a global index, just return the data as-is.
            if (!needToFix && !GlobalIndex.HasValue)
            {
                return base.Open();
            }
            
            var data = new MemoryStream(Length + 16);
            using (var writer = data.AsBinaryWriter())
            {
                // Add a GBIX header
                if (!GlobalIndex.HasValue)
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

                // Fix the texture lengths for the textures that need to be "fixed"
                if (needToFix)
                {
                    if (GlobalIndex.HasValue)
                    {
                        data.Position = 0x14;
                    }
                    else
                    {
                        data.Position = 0x4;
                    }

                    uint actualLength = PTStream.ReadUInt32(data);
                    data.Position -= 4;
                    PTStream.WriteUInt32(data, actualLength - 16);
                }
            }

            data.Seek(0, SeekOrigin.Begin);

            return data;
        }
    }
}
