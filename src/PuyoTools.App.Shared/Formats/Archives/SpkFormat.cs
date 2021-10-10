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
    internal partial class SpkFormat : IArchiveFormat
    {
        private SpkFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static SpkFormat Instance { get; } = new SpkFormat();

        public string Name => "SPK (Puyo Puyo Fever 2)";

        public string FileExtension => ".spk";

        public ArchiveBase GetCodec() => new SpkArchive();

        public bool Identify(Stream source, string filename) => SpkArchive.Identify(source);
    }
}
