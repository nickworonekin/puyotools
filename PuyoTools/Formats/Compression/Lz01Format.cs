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
    internal class Lz01Format : ICompressionFormat
    {
        private Lz01Format() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static Lz01Format Instance { get; } = new Lz01Format();

        public string Name => "LZ01";

        public CompressionBase GetCodec() => new Lz01Compression();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
