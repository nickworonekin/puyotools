using PuyoTools.App.Formats.Compression;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Compression
{
    class CompressionCompressCommand : Command
    {
        public CompressionCompressCommand()
            : base("compress", "Compress files")
        {
            // Add commands for all compression formats that can be used to compress files.
            foreach (var format in CompressionFactory.EncoderFormats)
            {
                AddCommand(format.GetCompressCommand());
            }
        }
    }
}
