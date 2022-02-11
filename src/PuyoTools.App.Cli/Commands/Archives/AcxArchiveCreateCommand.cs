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
    class AcxArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        public AcxArchiveCreateCommand(AcxFormat format)
            : base(format)
        {
            AddOption(new Option<int>("--block-size", () => 2048, "Set the block size")
                .FromAmong(new int[] { 4, 2048 }.Select(x => x.ToString()).ToArray()));

            Handler = CommandHandler.Create<AcxArchiveCreateOptions, IConsole>(Execute);
        }

        private void Execute(AcxArchiveCreateOptions options, IConsole console) => base.Execute(options, console);
    }
}
