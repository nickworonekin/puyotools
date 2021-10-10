using PuyoTools.App.Formats.Archives;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class AcxArchiveCreateOptions : ArchiveCreateOptions, IArchiveFormatOptions
    {
        public int BlockSize { get; set; }

        public void MapTo(ArchiveWriter obj)
        {
            var archive = (AcxArchiveWriter)obj;

            archive.BlockSize = BlockSize;
        }
    }
}
