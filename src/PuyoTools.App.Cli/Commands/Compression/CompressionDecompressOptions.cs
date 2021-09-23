using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Compression
{
    class CompressionDecompressOptions
    {
        public string[] Input { get; set; }

        public string[] Exclude { get; set; }

        public bool Overwrite { get; set; }

        public bool Delete { get; set; }

        public bool Quiet { get; set; }
    }
}
