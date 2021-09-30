using PuyoTools.App.Formats.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class PvmArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        public PvmArchiveCreateCommand(PvmFormat format)
            : base(format)
        {
            AddOption(new Option("--filenames", "Include filenames"));
            AddOption(new Option("--global-indexes", "Include global indexes"));
            AddOption(new Option("--formats", "Include pixel & data formats"));
            AddOption(new Option("--dimensions", "Include texture dimensions"));

            Handler = CommandHandler.Create<PvmArchiveCreateOptions, IConsole>(Execute);
        }

        private void Execute(PvmArchiveCreateOptions options, IConsole console) => base.Execute(options, console);
    }
}
