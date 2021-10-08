using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class ArchiveExtractOptions
    {
        public string[] Input { get; set; }

        public string[] Exclude { get; set; }

        public bool Decompress { get; set; }

        public bool ExtractSourceFolder { get; set; }

        public bool ExtractSameName { get; set; }

        public bool Delete { get; set; }

        public bool DecompressExtracted { get; set; }

        public bool FileNumber { get; set; }

        public bool PrependFileNumber { get; set; }

        public bool ExtractIfArchive { get; set; }

        public bool DecodeIfTexture { get; set; }

        public bool Verbose { get; set; }
    }
}
