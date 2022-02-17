using PuyoTools.Core.Textures.Gim;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class GimTextureEncodeOptions : TextureFormatEncodeOptions, ITextureFormatOptions
    {
        public GimPaletteFormat PaletteFormat { get; set; }

        public GimPixelFormat PixelFormat { get; set; }

        public bool Metadata { get; set; }

        public bool Swizzle { get; set; }

        public bool Dither { get; set; }

        public void MapTo(TextureBase obj)
        {
            var texture = (GimTexture)obj;

            texture.PaletteFormat = PaletteFormat;
            texture.DataFormat = PixelFormat;
            texture.HasMetadata = Metadata;
            texture.Swizzle = Swizzle;
            texture.Dither = Dither;
        }
    }
}
