using PuyoTools.App.Cli.Commands.Archives;
using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Archives
{
    /// <inheritdoc/>
    internal partial class OneStorybookFormat : IArchiveFormat
    {
        public string CommandName => "one-storybook";

        public ArchiveFormatCreateCommand GetCreateCommand() => new ArchiveFormatCreateCommand(this);
    }
}
