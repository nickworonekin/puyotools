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

        public bool Identify(Stream source, string filename) => TexArchive.Identify(source);
    }
}
