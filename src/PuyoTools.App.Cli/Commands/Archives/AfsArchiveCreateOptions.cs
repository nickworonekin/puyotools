using PuyoTools.App.Formats.Archives;
using PuyoTools.Archives.Formats.Afs;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    record AfsArchiveCreateOptions : ArchiveCreateOptions, IArchiveFormatOptions, IArchiveWriterOptions<AfsWriter>
    {
        public AfsArchiveCreateOptions(ParseResult parseResult) : base(parseResult)
        {
            AfsArchiveCreateCommand command = (AfsArchiveCreateCommand)parseResult.CommandResult.Command;

            BlockSize = parseResult.GetValue(command.BlockSizeOption);
            Version = parseResult.GetValue(command.VersionOption);
            Timestamps = parseResult.GetValue(command.TimestampsOption);
        }

        public int BlockSize { get; set; }

        public int Version { get; set; }

        public bool Timestamps { get; set; }

        public void MapTo(LegacyArchiveWriter obj)
        {
            var archive = (AfsArchiveWriter)obj;

            archive.BlockSize = BlockSize;
            archive.Version = Version == 2
                ? AfsArchiveWriter.AfsVersion.Version2
                : AfsArchiveWriter.AfsVersion.Version1;
            archive.HasTimestamps = Timestamps;
        }

        public void MapTo(AfsWriter obj)
        {
            obj.BlockSize = BlockSize;
            obj.Version = Version == 2
                ? AfsVersion.Version2
                : AfsVersion.Version1;
            obj.HasTimestamps = Timestamps;
        }
    }
}
