using PuyoTools.App.Cli.Commands.Archives;
using PuyoTools.Modules;
using PuyoTools.Modules.Archive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Archives
{
    /// <inheritdoc/>
    internal partial class PvmFormat : IArchiveFormat
    {
        public string CommandName => "pvm";

        public ArchiveFormatCreateCommand GetCreateCommand() => new PvmArchiveCreateCommand(this);
    }
}
