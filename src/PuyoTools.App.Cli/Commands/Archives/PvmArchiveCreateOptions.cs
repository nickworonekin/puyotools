using PuyoTools.App.Formats.Archives;
using PuyoTools.Archives.Formats.Pvm;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    record PvmArchiveCreateOptions : ArchiveCreateOptions, IArchiveFormatOptions, IArchiveWriterOptions<PvmWriter>
    {
        public PvmArchiveCreateOptions(ParseResult parseResult) : base(parseResult)
        {
            PvmArchiveCreateCommand command = (PvmArchiveCreateCommand)parseResult.CommandResult.Command;

            Filenames = parseResult.GetValue(command.FilenamesOption);
            GlobalIndexes = parseResult.GetValue(command.GlobalIndexesOption);
            Formats = parseResult.GetValue(command.FormatsOption);
            Dimensions = parseResult.GetValue(command.DimensionsOption);
        }

        public bool Filenames { get; }

        public bool GlobalIndexes { get; }

        public bool Formats { get; }

        public bool Dimensions { get; }

        public void MapTo(LegacyArchiveWriter obj)
        {
            var archive = (PvmArchiveWriter)obj;

            archive.HasFilenames = Filenames;
            archive.HasGlobalIndexes = GlobalIndexes;
            archive.HasFormats = Formats;
            archive.HasDimensions = Dimensions;
        }

        public void MapTo(PvmWriter obj)
        {
            obj.HasFilenames = Filenames;
            obj.HasGlobalIndexes = GlobalIndexes;
            obj.HasFormats = Formats;
            obj.HasDimensions = Dimensions;
        }
    }
}
