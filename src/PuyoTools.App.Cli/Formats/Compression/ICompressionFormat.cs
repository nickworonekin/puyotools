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
    internal partial interface ICompressionFormat
    {
        /// <summary>
        /// Gets the command name of this compression format.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Gets the compress command for this compression format.
        /// </summary>
        /// <returns>The compress command.</returns>
        CompressionFormatCompressCommand GetCompressCommand();
    }
}
