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

        public Option<int> BlockSizeOption => _blockSizeOption;

        public Option<int> VersionOption => _versionOption;

        public Option <bool> TimestampsOption => _timestampsOption;

        public AfsArchiveCreateCommand(AfsFormat format)
            : base(format)
        {
            _blockSizeOption = new Option<int>("--block-size")
            {
                Description = "Set the block size",
                DefaultValueFactory = _ => 2048,
            }.AcceptOnlyFromAmong(["16", "2048"]);
            Add(_blockSizeOption);

            _versionOption = new Option<int>("--version")
            {
                Description = "Set the AFS version",
                DefaultValueFactory = _ => 1,
            }
                .AcceptOnlyFromAmong(["1", "2"]);
            Add(_versionOption);

            _timestampsOption = new("--timestamps")
            {
                Description = "Include timestamp info",
            };
            Add(_timestampsOption);
        }

        protected override ArchiveCreateOptions CreateOptions(ParseResult parseResult)
            => new AfsArchiveCreateOptions(parseResult);
    }
}
