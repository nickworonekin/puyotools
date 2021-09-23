using PuyoTools.App.Formats.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class ArchiveCreateCommand : Command
    {
        public ArchiveCreateCommand()
            : base("create", "Create an archive")
        {
            // Add commands for all archive formats that can be used to create archives.
            foreach (var format in ArchiveFactory.WriterFormats)
            {
                AddCommand(format.GetCreateCommand());
            }
        }
    }
}
