using PuyoTools.App.Formats.Archives;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class AfsArchiveCreateOptions : ArchiveCreateOptions, IArchiveFormatOptions
    {
        public int BlockSize { get; set; }

        public int Version { get; set; }

        public bool Timestamps { get; set; }

        public void MapTo(ArchiveWriter obj)
        {
            var archive = (AfsArchiveWriter)obj;

            archive.BlockSize = BlockSize;
            archive.Version = Version == 2
                ? AfsArchiveWriter.AfsVersion.Version2
                : AfsArchiveWriter.AfsVersion.Version1;
            archive.HasTimestamps = Timestamps;
        }
    }
}
