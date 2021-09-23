using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class TextureDecodeOptions
    {
        public string[] Input { get; set; }

        public string[] Exclude { get; set; }

        public bool Compressed { get; set; }

        public bool Overwrite { get; set; }

        public bool Delete { get; set; }
    }
}
