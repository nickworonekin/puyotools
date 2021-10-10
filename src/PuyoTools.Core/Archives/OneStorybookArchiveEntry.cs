using PuyoTools.Core.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class OneStorybookArchiveEntry : ArchiveEntry
    {
        internal OneStorybookArchiveEntry(ArchiveReader archiveReader, long offset, int length, string name)
            : base(archiveReader, offset, length, name)
        {
        }

        /// <inheritdoc/>
        public override Stream Open()
        {
            var prsCompression = new PrsCompression();
            var data = new MemoryStream();
            using (var source = new StreamView(archiveReader.ArchiveStream, offset, length))
            {
                prsCompression.Decompress(source, data);
            }

            data.Seek(0, SeekOrigin.Begin);

            return data;
        }
    }
}
