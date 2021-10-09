using PuyoTools.Core.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Compression
{
    /// <inheritdoc/>
    internal partial class CxlzFormat : ICompressionFormat
    {
        private CxlzFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static CxlzFormat Instance { get; } = new CxlzFormat();

        public string Name => "CXLZ";

        public CompressionBase GetCodec() => new CxlzCompression();

        public bool Identify(Stream source, string filename) => CxlzCompression.Identify(source);
    }
}
