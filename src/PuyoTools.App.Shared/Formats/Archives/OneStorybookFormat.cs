using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Storybook;
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
    internal partial class OneStorybookFormat : IArchiveFormat
    {
        private OneStorybookFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static OneStorybookFormat Instance { get; } = new OneStorybookFormat();

        public string Name => "ONE (Sonic Storybook series)";

        public string FileExtension => ".one";

        public ArchiveBase GetCodec() => new OneStorybookArchive();

        public ArchiveReader CreateReader(Stream source) => new OneStorybookReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => null;

        public bool Identify(Stream source, string filename) => OneStorybookReader.IsFormat(source);
    }
}
