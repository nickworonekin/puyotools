using PuyoTools.App.Formats.Archives;
using PuyoTools.Archives.Formats.Snt;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    record SntArchiveCreateOptions : ArchiveCreateOptions, IArchiveFormatOptions, IArchiveWriterOptions<SntWriter>
    {
        public SntArchiveCreateOptions(ParseResult parseResult) : base(parseResult)
        {
            SntArchiveCreateCommand command = (SntArchiveCreateCommand)parseResult.CommandResult.Command;

            Platform = parseResult.GetValue(command.PlatformOption);
        }

        public SntPlatform Platform { get; }

        public void MapTo(LegacyArchiveWriter obj)
        {
            var archive = (SntArchiveWriter)obj;

            archive.Platform = Platform == SntPlatform.PlayStationPortable
                ? SntArchiveWriter.SntPlatform.Psp
                : SntArchiveWriter.SntPlatform.Ps2;
        }

        public void MapTo(SntWriter obj)
        {
            obj.Platform = Platform;
        }
    }
}