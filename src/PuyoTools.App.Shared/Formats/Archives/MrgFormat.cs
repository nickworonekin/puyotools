using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Mrg;
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
    internal partial class MrgFormat : IArchiveFormat
    {
        private MrgFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static MrgFormat Instance { get; } = new MrgFormat();

        public string Name => "MRG (Puyo Puyo Fever 2)";

        public string FileExtension => ".mrg";

        public ArchiveBase GetCodec() => new MrgArchive();

        public ArchiveReader CreateReader(Stream source) => new MrgReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => null;

        public bool Identify(Stream source, string filename) => MrgReader.IsFormat(source);
    }
}
