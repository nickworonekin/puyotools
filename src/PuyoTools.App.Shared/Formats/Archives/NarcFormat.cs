using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Narc;
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
    internal partial class NarcFormat : IArchiveFormat
    {
        private NarcFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static NarcFormat Instance { get; } = new NarcFormat();

        public string Name => "NARC";

        public string FileExtension => ".narc";

        public ArchiveBase GetCodec() => new NarcArchive();

        public ArchiveReader CreateReader(Stream source) => new NarcReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => new NarcWriter(destination);

        public bool Identify(Stream source, string filename) => NarcReader.IsFormat(source);
    }
}
