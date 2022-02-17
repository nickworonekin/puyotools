using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.Core.Textures.Gvr;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class GvrTextureEncodeOptions : TextureFormatEncodeOptions, ITextureFormatOptions
    {
        public GvrPixelFormat PaletteFormat { get; set; }

        public GvrDataFormat PixelFormat { get; set; }

        public uint? GlobalIndex { get; set; }

        public bool Gcix { get; set; }

        public bool Dither { get; set; }

        public void MapTo(TextureBase obj)
        {
            var texture = (GvrTexture)obj;

            texture.PaletteFormat = PaletteFormat;
            texture.DataFormat = PixelFormat;
            texture.GbixType = Gcix ? GvrGbixType.Gcix : GvrGbixType.Gbix;
            texture.HasGlobalIndex = GlobalIndex.HasValue;
            texture.GlobalIndex = GlobalIndex ?? default;
            texture.Dither = Dither;
        }
    }
}
