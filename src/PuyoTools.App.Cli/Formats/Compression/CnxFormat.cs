using PuyoTools.App.Cli.Commands.Compression;
using PuyoTools.Core;
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
    internal partial class CnxFormat : ICompressionFormat
    {
        public string CommandName => "cnx";

        public CompressionFormatCompressCommand GetCompressCommand() => new CompressionFormatCompressCommand(this);
    }
}
