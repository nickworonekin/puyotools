using PuyoTools.Core.Textures.Gim;
using PuyoTools.App.Formats.Textures;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class GimTextureEncodeCommand : TextureFormatEncodeCommand
    {
        public GimTextureEncodeCommand(GimFormat format)
            : base(format)
        {
            AddOption(new Option<GimPaletteFormat>("--palette-format", "Set the palette format"));
            AddOption(new Option<GimPixelFormat>("--pixel-format", "Set the pixel format")
            {
                IsRequired = true,
            });
            AddOption(new Option<bool>("--metadata", "Include metadata"));
            AddOption(new Option<bool>("--swizzle", "Use texture swizzling (for PSP only)."));
            AddOption(new Option<bool>("--dither", "Use dithering when creating palette-based textures."));

            Handler = CommandHandler.Create<GimTextureEncodeOptions, IConsole>(Execute);
        }

        private void Execute(GimTextureEncodeOptions options, IConsole console) => base.Execute(options, console);
    }
}
