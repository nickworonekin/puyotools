using System;
using System.Collections.Generic;
using System.Text;
using PuyoTools.App.Cli.Commands.Archives;
using PuyoTools.App.Formats.Archives;

namespace PuyoTools.App.Formats.Archives
{
    /// <inheritdoc/>
    internal partial class VddFormat : IArchiveFormat
    {
        public string CommandName => "vdd";

        public ArchiveFormatCreateCommand GetCreateCommand() => new ArchiveFormatCreateCommand(this);
    }
}
