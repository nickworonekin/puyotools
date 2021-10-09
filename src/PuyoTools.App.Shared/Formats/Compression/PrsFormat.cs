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
    internal partial class PrsFormat : ICompressionFormat
    {
        private PrsFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static PrsFormat Instance { get; } = new PrsFormat();

        public string Name => "PRS";

        public CompressionBase GetCodec() => new PrsCompression();

        public bool Identify(Stream source, string filename) => Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase) && PrsCompression.Identify(source);
    }
}
