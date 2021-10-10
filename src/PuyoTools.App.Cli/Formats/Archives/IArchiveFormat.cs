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
    internal partial interface IArchiveFormat
    {
        /// <summary>
        /// Gets the command name of this archive format.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Gets the create command for this compression format.
        /// </summary>
        /// <returns>The create command.</returns>
        ArchiveFormatCreateCommand GetCreateCommand();
    }
}
