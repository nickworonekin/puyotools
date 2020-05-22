﻿using PuyoTools.Modules.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Formats.Compression
{
    /// <inheritdoc/>
    internal class Lz00Format : ICompressionFormat
    {
        private Lz00Format() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static Lz00Format Instance { get; } = new Lz00Format();

        public string Name => "LZ00";

        public CompressionBase GetCodec() => new Lz00Compression();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}