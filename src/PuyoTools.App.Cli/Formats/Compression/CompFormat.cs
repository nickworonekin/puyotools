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
    internal partial class CompFormat : ICompressionFormat
    {
        public string CommandName => "comp";

        public CompressionFormatCompressCommand GetCompressCommand() => new CompressionFormatCompressCommand(this);
    }
}
