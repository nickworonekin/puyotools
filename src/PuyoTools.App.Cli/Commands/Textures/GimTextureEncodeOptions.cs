using GimSharp;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GimTexture = PuyoTools.Core.Texture.GimTexture;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class GimTextureEncodeOptions : TextureFormatEncodeOptions, ITextureFormatOptions
    {
        public GimPaletteFormat PaletteFormat { get; set; }

        public GimDataFormat DataFormat { get; set; }

        public bool Metadata { get; set; }

        public void MapTo(TextureBase obj)
        {
            var texture = (GimTexture)obj;

            texture.PaletteFormat = PaletteFormat;
            texture.DataFormat = DataFormat;
            texture.HasMetadata = Metadata;
        }
    }
}
