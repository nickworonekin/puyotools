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
    internal partial class CompFormat : ICompressionFormat
    {
        private CompFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static CompFormat Instance { get; } = new CompFormat();

        public string Name => "COMP (Puyo Puyo Chronicle)";

        public CompressionBase GetCodec() => new CompCompression();

        public bool Identify(Stream source, string filename) => CompCompression.Identify(source);
    }
}
