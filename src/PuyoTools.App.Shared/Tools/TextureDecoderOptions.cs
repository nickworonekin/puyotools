using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.App.Tools
{
    class TextureDecoderOptions
    {
        public bool DecodeCompressedTextures { get; set; }

        public bool OutputToSourceDirectory { get; set; }

        public bool DeleteSource { get; set; }
    }
}
