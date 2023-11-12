using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Gnt;
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
    internal partial class GntFormat : IArchiveFormat
    {
        private GntFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static GntFormat Instance { get; } = new GntFormat();

        public string Name => "GNT";

        public string FileExtension => ".gnt";

        public ArchiveBase GetCodec() => new GntArchive();

        public ArchiveReader CreateReader(Stream source) => new GntReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => null;

        public bool Identify(Stream source, string filename) => GntReader.IsFormat(source);
    }
}
