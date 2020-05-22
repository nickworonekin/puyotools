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
    internal class PrsFormat : ICompressionFormat
    {
        private PrsFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static PrsFormat Instance { get; } = new PrsFormat();

        public string Name => "PRS";

        public CompressionBase GetCodec() => new PrsCompression();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
