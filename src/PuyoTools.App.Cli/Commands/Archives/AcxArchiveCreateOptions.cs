using PuyoTools.App.Formats.Archives;
using PuyoTools.Archives.Formats.Acx;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    record AcxArchiveCreateOptions : ArchiveCreateOptions, IArchiveFormatOptions, IArchiveWriterOptions<AcxWriter>
    {
        public AcxArchiveCreateOptions(ParseResult parseResult) : base(parseResult)
        {
            AcxArchiveCreateCommand command = (AcxArchiveCreateCommand)parseResult.CommandResult.Command;

            BlockSize = parseResult.GetValue(command.BlockSizeOption);
        }

        public int BlockSize { get; }

        public void MapTo(LegacyArchiveWriter obj)
        {
            var archive = (AcxArchiveWriter)obj;

            archive.BlockSize = BlockSize;
        }

        public void MapTo(AcxWriter obj)
        {
            obj.BlockSize = BlockSize;
        }
    }
}
