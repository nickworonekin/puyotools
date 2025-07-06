using PuyoTools.App.Formats.Archives;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class SntArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        private readonly Option<SntArchiveWriter.SntPlatform> _platformOption;

        public SntArchiveCreateCommand(SntFormat format)
            : base(format)
        {
            _platformOption = new("--platform")
            {
                Description = "Platform this archive will be used on",
                DefaultValueFactory = _ => SntArchiveWriter.SntPlatform.Ps2,
            };
            Add(_platformOption);
        }

        protected override ArchiveCreateOptions CreateOptions(ParseResult parseResult)
        {
            SntArchiveCreateOptions options = new()
            {
                Platform = parseResult.GetValue(_platformOption),
            };

            SetBaseOptions(parseResult, options);

            return options;
        }
    }
}
