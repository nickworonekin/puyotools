using PuyoTools.App.Formats.Textures;
using PuyoTools.Modules.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrSharp.Svr;
using SvrTexture = PuyoTools.Modules.Texture.SvrTexture;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class SvrTextureEncodeOptions : TextureFormatEncodeOptions, ITextureFormatOptions
    {
        public SvrPixelFormat PixelFormat { get; set; }

        public SvrDataFormat DataFormat { get; set; }

        public uint? GlobalIndex { get; set; }

        public void MapTo(TextureBase obj)
        {
            var texture = (SvrTexture)obj;

            texture.PixelFormat = PixelFormat;
            texture.DataFormat = DataFormat;
            texture.HasGlobalIndex = GlobalIndex.HasValue;
            texture.GlobalIndex = GlobalIndex ?? default;
        }
    }
}
