using PuyoTools.App.Formats.Textures;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.Core.Textures.Gvr;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class GvrTextureEncodeCommand : TextureFormatEncodeCommand
    {
        public GvrTextureEncodeCommand(GvrFormat format)
            : base(format)
        {
            var paletteFormatOption = new Option<GvrPixelFormat>("--palette-format", "Set the palette format");
            var pixelFormatOption = new Option<GvrDataFormat>("--pixel-format", "Set the pixel format")
            {
                IsRequired = true,
            };

            AddOption(paletteFormatOption);
            AddOption(pixelFormatOption);
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
            AddOption(new Option<bool>("--gcix", "Use GCIX global index header."));
            AddOption(new Option<bool>("--dither", "Use dithering when creating palette-based textures."));

            AddValidator(result =>
            {
                // If the pixel format is palette-based, validate that a palette format was passed.
                var pixelFormat = result.GetValueForOption(pixelFormatOption);
                var paletteFormatResult = result.FindResultFor(paletteFormatOption);

                if (pixelFormat is GvrDataFormat.Index4 or GvrDataFormat.Index8
                    && paletteFormatResult is null)
                {
                    result.ErrorMessage = $"Palette format is required for pixel format '{pixelFormat}'.";
                }
            });

            Handler = CommandHandler.Create<GvrTextureEncodeOptions, IConsole>(Execute);
        }

        private void Execute(GvrTextureEncodeOptions options, IConsole console) => base.Execute(options, console);
    }
}
