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
    class GvmArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        public GvmArchiveCreateCommand(GvmFormat format)
            : base(format)
        {
            AddOption(new Option("--filenames", "Include filenames"));
            AddOption(new Option("--global-indexes", "Include global indexes"));
            AddOption(new Option("--formats", "Include pixel & data formats"));
            AddOption(new Option("--dimensions", "Include texture dimensions"));

            Handler = CommandHandler.Create<GvmArchiveCreateOptions, IConsole>(Execute);
        }

        private void Execute(GvmArchiveCreateOptions options, IConsole console) => base.Execute(options, console);
    }
}
