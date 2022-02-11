using PuyoTools.App.Formats.Archives;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class SntArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        public SntArchiveCreateCommand(SntFormat format)
            : base(format)
        {
            AddOption(new Option<SntArchiveWriter.SntPlatform>("--platform", () => SntArchiveWriter.SntPlatform.Ps2, "Platform this archive will be used on"));

            Handler = CommandHandler.Create<SntArchiveCreateOptions, IConsole>(Execute);
        }

        private void Execute(SntArchiveCreateOptions options, IConsole console) => base.Execute(options, console);
    }
}
