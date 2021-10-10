using PuyoTools.Core.Textures.Gim;
using PuyoTools.App.Formats.Textures;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
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
            AddOption(new Option<GimDataFormat>("--data-format", "Set the data format")
            {
                IsRequired = true,
            });
            AddOption(new Option("--metadata", "Include metadata"));

            Handler = CommandHandler.Create<GimTextureEncodeOptions, IConsole>(Execute);
        }

        private void Execute(GimTextureEncodeOptions options, IConsole console) => base.Execute(options, console);
    }
}
