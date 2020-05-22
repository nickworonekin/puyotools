using PuyoTools.Modules.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Formats.Compression
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class Lz10Format : ICompressionFormat
    {
        private Lz10Format() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static Lz10Format Instance { get; } = new Lz10Format();

        public string Name => "LZ10/LZSS";

        public CompressionBase GetCodec() => new Lz10Compression();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
