using PuyoTools.Modules.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Formats.Compression
{
    /// <inheritdoc/>
    internal class CompFormat : ICompressionFormat
    {
        private CompFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static CompFormat Instance { get; } = new CompFormat();

        public string Name => "COMP (Puyo Puyo Chronicle)";

        public CompressionBase GetCodec() => new CompCompression();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
