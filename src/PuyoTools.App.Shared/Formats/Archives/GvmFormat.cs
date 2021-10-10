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
    internal partial class GvmFormat : IArchiveFormat
    {
        private GvmFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static GvmFormat Instance { get; } = new GvmFormat();

        public string Name => "GVM";

        public string FileExtension => ".gvm";

        public ArchiveBase GetCodec() => new GvmArchive();

        public bool Identify(Stream source, string filename) => GvmArchive.Identify(source);
    }
}
