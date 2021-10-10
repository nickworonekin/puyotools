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
    internal partial class SntFormat : IArchiveFormat
    {
        private SntFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static SntFormat Instance { get; } = new SntFormat();

        public string Name => "SNT";

        public string FileExtension => ".Snt";

        public ArchiveBase GetCodec() => new SntArchive();

        public bool Identify(Stream source, string filename) => SntArchive.Identify(source);
    }
}
