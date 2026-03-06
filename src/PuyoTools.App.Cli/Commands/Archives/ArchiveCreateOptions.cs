using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    record ArchiveCreateOptions
    {
        public ArchiveCreateOptions(ParseResult parseResult)
        {
            ArchiveFormatCreateCommand command = (ArchiveFormatCreateCommand)parseResult.CommandResult.Command;

            Input = parseResult.GetRequiredValue(command.InputOption);
            Exclude = parseResult.GetValue(command.ExcludeOption);
            Output = parseResult.GetRequiredValue(command.OutputOption);
            Compress = parseResult.GetValue(command.CompressOption);
        }

        public string[] Input { get; }

        public string[]? Exclude { get; }

        public string Output { get; }

        public string? Compress { get; }
    }
}
