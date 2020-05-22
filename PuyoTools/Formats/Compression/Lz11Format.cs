﻿using PuyoTools.Modules.Compression;
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
    internal class Lz11Format : ICompressionFormat
    {
        private Lz11Format() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static Lz11Format Instance { get; } = new Lz11Format();

        public string Name => "LZ11";

        public CompressionBase GetCodec() => new Lz11Compression();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
