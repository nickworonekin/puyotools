using PuyoTools.App.Cli.Commands.Compression;
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
    internal partial class Lz11Format : ICompressionFormat
    {
        public string CommandName => "lz11";

        public CompressionFormatCompressCommand GetCompressCommand() => new CompressionFormatCompressCommand(this);
    }
}
