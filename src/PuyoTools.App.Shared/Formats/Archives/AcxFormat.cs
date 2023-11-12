using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Acx;
using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Archives
{
    /// <inheritdoc/>
    internal partial class AcxFormat : IArchiveFormat
    {
        private AcxFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static AcxFormat Instance { get; } = new AcxFormat();

        public string Name => "ACX";

        public string FileExtension => ".acx";

        public ArchiveBase GetCodec() => new AcxArchive();

        public ArchiveReader CreateReader(Stream source) => new AcxReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => new AcxWriter(destination);

        public bool Identify(Stream source, string filename) => Path.GetExtension(filename).Equals(FileExtension, StringComparison.OrdinalIgnoreCase) && AcxReader.IsFormat(source);
    }
}
