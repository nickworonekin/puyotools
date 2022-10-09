using PuyoTools.Core.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Storybook
{
    public class OneStorybookReaderEntry : ArchiveReaderEntry
    {
        public OneStorybookReaderEntry(Stream stream, long position, long length, long uncompressedLength, string name)
            : base(stream, position, length, name)
        {
            UncompressedLength = uncompressedLength;
        }

        public long UncompressedLength { get; }

        public override Stream Open()
        {
            MemoryStream stream = new((int)UncompressedLength);
            PrsCompression prsDecoder = new();
            prsDecoder.Decompress(base.Open(), stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
