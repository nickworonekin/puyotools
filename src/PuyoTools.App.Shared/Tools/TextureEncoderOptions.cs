using PuyoTools.App.Formats.Compression;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.App.Tools
{
    class TextureEncoderOptions
    {
        public ICompressionFormat CompressionFormat { get; set; }

        public bool OutputToSourceDirectory { get; set; }

        public bool DeleteSource { get; set; }
    }
}
