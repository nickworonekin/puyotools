using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Tex;
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
    internal partial class TexFormat : IArchiveFormat
    {
        private TexFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static TexFormat Instance { get; } = new TexFormat();

        public string Name => "TEX (Puyo Puyo Fever 2)";

        public string FileExtension => ".tex";

        public ArchiveBase GetCodec() => new TexArchive();

        public ArchiveReader CreateReader(Stream source) => new TexReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => null;

        public bool Identify(Stream source, string filename) => TexReader.IsFormat(source);
    }
}
