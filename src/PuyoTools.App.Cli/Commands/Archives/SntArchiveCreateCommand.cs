using PuyoTools.App.Formats.Archives;
using PuyoTools.Archives.Formats.Snt;
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
        private readonly Option<SntPlatform> _platformOption;

        public Option<SntPlatform> PlatformOption => _platformOption;

        public SntArchiveCreateCommand(SntFormat format)
            : base(format)
        {
            _platformOption = new("--platform")
            {
                Description = "Platform this archive will be used on",
                DefaultValueFactory = _ => SntPlatform.PlayStation2,
            };
            Add(_platformOption);
        }

        protected override ArchiveCreateOptions CreateOptions(ParseResult parseResult)
            => new SntArchiveCreateOptions(parseResult);
    }
}
