using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Svm;
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
    internal partial class SvmFormat : IArchiveFormat
    {
        private SvmFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static SvmFormat Instance { get; } = new SvmFormat();

        public string Name => "SVM";

        public string FileExtension => ".svm";

        public ArchiveBase GetCodec() => new SvmArchive();

        public ArchiveReader CreateReader(Stream source) => new SvmReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => null;

        public bool Identify(Stream source, string filename) => SvmReader.IsFormat(source);
    }
}
