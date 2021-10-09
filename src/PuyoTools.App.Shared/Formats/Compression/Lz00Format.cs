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
    internal partial class Lz00Format : ICompressionFormat
    {
        private Lz00Format() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static Lz00Format Instance { get; } = new Lz00Format();

        public string Name => "LZ00";

        public CompressionBase GetCodec() => new Lz00Compression();

        public bool Identify(Stream source, string filename) => Lz00Compression.Identify(source);
    }
}
