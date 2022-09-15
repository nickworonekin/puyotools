using PuyoTools.App.Formats.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
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
            AddOption(new Option<bool>("--filenames", "Include filenames"));
            AddOption(new Option<bool>("--global-indexes", "Include global indexes"));
            AddOption(new Option<bool>("--formats", "Include pixel & data formats"));
            AddOption(new Option<bool>("--dimensions", "Include texture dimensions"));

            Handler = CommandHandler.Create<GvmArchiveCreateOptions, IConsole>(Execute);
        }

        private void Execute(GvmArchiveCreateOptions options, IConsole console) => base.Execute(options, console);
    }
}
