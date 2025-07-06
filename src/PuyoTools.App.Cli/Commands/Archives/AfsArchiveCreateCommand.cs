using PuyoTools.App.Formats.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class AfsArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        private readonly Option<int> _blockSizeOption;
        private readonly Option<int> _versionOption;
        private readonly Option<bool> _timestampsOption;

        public AfsArchiveCreateCommand(AfsFormat format)
            : base(format)
        {
            _blockSizeOption = new Option<int>("--block-size")
            {
                Description = "Set the block size",
                DefaultValueFactory = _ => 2048,
            }
                .AcceptOnlyFromAmong(new int[] { 16, 2048 }.Select(x => x.ToString()).ToArray());
            Add(_blockSizeOption);

            _versionOption = new Option<int>("--version")
            {
                Description = "Set the AFS version",
                DefaultValueFactory = _ => 1,
            }
                .AcceptOnlyFromAmong(new int[] { 1, 2 }.Select(x => x.ToString()).ToArray());
            Add(_versionOption);

            _timestampsOption = new("--timestamps")
            {
                Description = "Include timestamp info",
            };
            Add(_timestampsOption);
        }

        protected override ArchiveCreateOptions CreateOptions(ParseResult parseResult)
        {
            AfsArchiveCreateOptions options = new()
            {
                BlockSize = parseResult.GetValue(_blockSizeOption),
                Version = parseResult.GetValue(_versionOption),
                Timestamps = parseResult.GetValue(_timestampsOption),
            };

            SetBaseOptions(parseResult, options);

            return options;
        }
    }
}
