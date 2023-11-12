using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Unleashed;
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
    internal partial class OneUnleashedFormat : IArchiveFormat
    {
        private OneUnleashedFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static OneUnleashedFormat Instance { get; } = new OneUnleashedFormat();

        public string Name => "ONE (Sonic Unleashed)";

        public string FileExtension => ".one";

        public ArchiveBase GetCodec() => new OneUnleashedArchive();

        public ArchiveReader CreateReader(Stream source) => new OneUnleashedReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => null;

        public bool Identify(Stream source, string filename) => OneUnleashedReader.IsFormat(source);
    }
}
