using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Archives
{
    class ArchiveCreateOptions
    {
        public string[] Input { get; set; }

        public string[] Exclude { get; set; }

        public string Output { get; set; }

        public string Compress { get; set; }
    }
}
