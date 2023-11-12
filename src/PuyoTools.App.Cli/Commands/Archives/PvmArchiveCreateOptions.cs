using PuyoTools.App.Formats.Archives;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class PvmArchiveCreateOptions : ArchiveCreateOptions, IArchiveFormatOptions
    {
        public bool Filenames { get; set; }

        public bool GlobalIndexes { get; set; }

        public bool Formats { get; set; }

        public bool Dimensions { get; set; }

        public void MapTo(LegacyArchiveWriter obj)
        {
            var archive = (PvmArchiveWriter)obj;

            archive.HasFilenames = Filenames;
            archive.HasGlobalIndexes = GlobalIndexes;
            archive.HasFormats = Formats;
            archive.HasDimensions = Dimensions;
        }
    }
}
