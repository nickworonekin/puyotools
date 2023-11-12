using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Snt;
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
    internal partial class SntFormat : IArchiveFormat
    {
        private SntFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static SntFormat Instance { get; } = new SntFormat();

        public string Name => "SNT";

        public string FileExtension => ".Snt";

        public ArchiveBase GetCodec() => new SntArchive();

        public ArchiveReader CreateReader(Stream source) => new SntReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => null;

        public bool Identify(Stream source, string filename) => SntReader.IsFormat(source);
    }
}
