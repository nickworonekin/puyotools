using PuyoTools.Core.Archives;
using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.Core.Compression;

namespace PuyoTools.Archives.Formats.Gvm
{
    public class GvmReaderEntry : ArchiveReaderEntry
    {
        private long _reportedLength;

        public GvmReaderEntry(Stream stream, long position, long length, string? name, uint? globalIndex, long reportedLength)
            : base(stream, position, length, name)
        {
            GlobalIndex = globalIndex;
            _reportedLength = reportedLength;
        }

        public uint? GlobalIndex { get; }

        public override Stream Open()
        {
            // If the texture associated with this entry does not have a global index,
            // and the reported length is correct, just return the data as-is.
            if (!GlobalIndex.HasValue && _length == _reportedLength)
            {
                return base.Open();
            }

            MemoryStream stream;
            if (GlobalIndex.HasValue)
            {
                stream = new MemoryStream((int)_length + 16);
            }
            else
            {
                stream = new MemoryStream((int)_length);
            }

            using BinaryWriter writer = new(stream, Encoding.UTF8, true);

            // Write the GBIX header.
            if (GlobalIndex.HasValue)
            {
                writer.Write(GvmConstants.GbixMagicCode);
                writer.WriteInt32(8);
                writer.WriteUInt32(GlobalIndex.Value);
                writer.WriteInt32(0);
            }

            long pvrtPosition = stream.Position;

            // Write the remaining texture data.
            base.Open().CopyTo(stream);

            // If the reported length of the texture doesn't match the actual length,
            // change it to the correct value.
            if (_length != _reportedLength)
            {
                writer.At(pvrtPosition + 0x4, x => x.WriteInt32((int)_length - 8));
            }

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
