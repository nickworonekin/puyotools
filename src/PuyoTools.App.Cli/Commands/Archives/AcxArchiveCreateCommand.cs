using PuyoTools.App.Formats.Archives;
using System;
using System.CommandLine;
using System.Linq;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class AcxArchiveCreateCommand : ArchiveFormatCreateCommand
    {
        private readonly Option<int> _blockSizeOption;

        public Option<int> BlockSizeOption => _blockSizeOption;

        public AcxArchiveCreateCommand(AcxFormat format)
            : base(format)
        {
            _blockSizeOption = new Option<int>("--block-size")
            {
                Description = "Set the block size",
                DefaultValueFactory = _ => 2048,
            }.AcceptOnlyFromAmong(["4", "2048"]);
            Add(_blockSizeOption);
        }

        protected override ArchiveCreateOptions CreateOptions(ParseResult parseResult)
            => new AcxArchiveCreateOptions(parseResult);
    }
}
