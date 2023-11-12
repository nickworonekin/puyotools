using PuyoTools.App.Formats.Archives;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class SntArchiveCreateOptions : ArchiveCreateOptions, IArchiveFormatOptions
    {
        public SntArchiveWriter.SntPlatform Platform { get; set; }

        public void MapTo(LegacyArchiveWriter obj)
        {
            var archive = (SntArchiveWriter)obj;

            archive.Platform = Platform;
        }
    }
}