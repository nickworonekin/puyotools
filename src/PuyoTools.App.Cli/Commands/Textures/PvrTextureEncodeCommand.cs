using PuyoTools.App.Formats.Textures;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.Core.Textures.Pvr;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class PvrTextureEncodeCommand : TextureFormatEncodeCommand
    {
        public PvrTextureEncodeCommand(PvrFormat format)
            : base(format)
        {
            AddOption(new Option<PvrPixelFormat>("--pixel-format", "Set the pixel format")
            {
                IsRequired = true,
            });
            AddOption(new Option<PvrDataFormat>("--data-format", "Set the data format")
            {
                IsRequired = true,
            });
            AddOption(new Option<uint?>("--global-index", "Adds the GBIX header, optionally with a global index.")
            {
                Arity = ArgumentArity.ZeroOrOne,
            });
            AddOption(new Option("--rle-compression", "RLE compression (PVZ) for Puyo Puyo Fever."));

            Handler = CommandHandler.Create<PvrTextureEncodeOptions, IConsole>(Execute);
        }

        private void Execute(PvrTextureEncodeOptions options, IConsole console) => base.Execute(options, console);
    }
}
