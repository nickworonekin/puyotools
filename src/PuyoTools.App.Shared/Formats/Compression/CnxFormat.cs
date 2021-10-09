using PuyoTools.Core;
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
    internal partial class CnxFormat : ICompressionFormat
    {
        private CnxFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static CnxFormat Instance { get; } = new CnxFormat();

        public string Name => "CNX";

        public CompressionBase GetCodec() => new CnxCompression();

        public bool Identify(Stream source, string filename) => CnxCompression.Identify(source);
    }
}
