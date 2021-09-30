using PuyoTools.App.Formats.Textures;
using PuyoTools.Modules.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrSharp.Pvr;
using PvrTexture = PuyoTools.Modules.Texture.PvrTexture;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class PvrTextureEncodeOptions : TextureFormatEncodeOptions, ITextureFormatOptions
    {
        public PvrPixelFormat PixelFormat { get; set; }

        public PvrDataFormat DataFormat { get; set; }

        public uint? GlobalIndex { get; set; }

        public void MapTo(TextureBase obj)
        {
            var texture = (PvrTexture)obj;

            texture.PixelFormat = PixelFormat;
            texture.DataFormat = DataFormat;
            texture.HasGlobalIndex = GlobalIndex.HasValue;
            texture.GlobalIndex = GlobalIndex ?? default;
        }
    }
}
