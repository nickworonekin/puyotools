using PuyoTools.App.Formats.Textures;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
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
            AddOption(new Option<uint?>("--global-index", result =>
            {
                // If the option was passed with an argument, use the argument's value as the global index.
                if (result.Tokens.Any()
                    && uint.TryParse(result.Tokens[0].Value, out uint globalIndex))
                {
                    return globalIndex;
                }

                // Otherwise, if the option was passed without an argument, use 0 as the global index.
                return 0;
            }, description: "Adds the GBIX header, optionally with a global index.")
            {
                Arity = ArgumentArity.ZeroOrOne,
            });
            AddOption(new Option<bool>("--rle-compression", "RLE compression (PVZ) for Puyo Puyo Fever."));
            AddOption(new Option<bool>("--dither", "Use dithering when creating palette-based textures."));

            Handler = CommandHandler.Create<PvrTextureEncodeOptions, IConsole>(Execute);
        }

        private void Execute(PvrTextureEncodeOptions options, IConsole console) => base.Execute(options, console);
    }
}
