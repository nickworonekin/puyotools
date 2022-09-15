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
    class AfsArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        public AfsArchiveCreateCommand(AfsFormat format)
            : base(format)
        {
            AddOption(new Option<int>("--block-size", () => 2048, "Set the block size")
                .FromAmong(new int[] { 16, 2048 }.Select(x => x.ToString()).ToArray()));
            AddOption(new Option<int>("--version", () => 1, "Set the AFS version")
                .FromAmong(new int[] { 1, 2 }.Select(x => x.ToString()).ToArray()));
            AddOption(new Option<bool>("--timestamps", "Include timestamp info"));

            Handler = CommandHandler.Create<AfsArchiveCreateOptions, IConsole>(Execute);
        }

        private void Execute(AfsArchiveCreateOptions options, IConsole console) => base.Execute(options, console);
    }
}
