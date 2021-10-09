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
    internal partial class Lz10Format : ICompressionFormat
    {
        private Lz10Format() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static Lz10Format Instance { get; } = new Lz10Format();

        public string Name => "LZ10/LZSS";

        public CompressionBase GetCodec() => new Lz10Compression();

        public bool Identify(Stream source, string filename) => Lz10Compression.Identify(source);
    }
}
