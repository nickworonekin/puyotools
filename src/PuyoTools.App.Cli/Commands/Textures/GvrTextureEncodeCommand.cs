using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures.Gvr;
using PuyoTools.Core.Textures.Svr;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class GvrTextureEncodeCommand : TextureFormatEncodeCommand
    {
        private readonly Option<GvrPixelFormat> _paletteFormatOption;
        private readonly Option<GvrDataFormat> _pixelFormatOption;
        private readonly Option<uint?> _globalIndexOption;
        private readonly Option<bool> _gcixOption;
        private readonly Option<bool> _ditherOption;

        public GvrTextureEncodeCommand(GvrFormat format)
            : base(format)
        {
            _paletteFormatOption = new("--palette-format")
            {
                Description = "Set the palette format",
            };
            Add(_paletteFormatOption);

            _pixelFormatOption = new("--pixel-format")
            {
                Description = "Set the pixel format",
                Required = true,
            };
            Add(_pixelFormatOption);

            _globalIndexOption = new("--global-index")
            {
                Description = "Adds the GBIX header, optionally with a global index.",
                Arity = ArgumentArity.ZeroOrOne,
                CustomParser = result =>
                {
                    // If the option was passed with an argument, use the argument's value as the global index.
                    if (result.Tokens.Any()
                        && uint.TryParse(result.Tokens[0].Value, out uint globalIndex))
                    {
                        return globalIndex;
                    }

                    // Otherwise, if the option was passed without an argument, use 0 as the global index.
                    return 0;
                },
            };
            Add(_globalIndexOption);

            _gcixOption = new("--gcix")
            {
                Description = "Use GCIX global index header.",
            };
            Add(_gcixOption);

            _ditherOption = new("--dither")
            {
                Description = "Use dithering when creating palette-based textures.",
            };
            Add(_ditherOption);

            Validators.Add(result =>
            {
                // If the pixel format is palette-based, validate that a palette format was passed.
                var pixelFormat = result.GetValue(_pixelFormatOption);
                var paletteFormatResult = result.GetResult(_paletteFormatOption);

                if (pixelFormat is GvrDataFormat.Index4 or GvrDataFormat.Index8
                    && paletteFormatResult is null)
                {
                    result.AddError($"Palette format is required for pixel format '{pixelFormat}'.");
                }
            });
        }

        protected override TextureFormatEncodeOptions CreateOptions(ParseResult parseResult)
        {
            GvrTextureEncodeOptions options = new()
            {
                PaletteFormat = parseResult.GetValue(_paletteFormatOption),
                PixelFormat = parseResult.GetValue(_pixelFormatOption),
                GlobalIndex = parseResult.GetValue(_globalIndexOption),
                Gcix = parseResult.GetValue(_gcixOption),
                Dither = parseResult.GetValue(_ditherOption),
            };

            SetBaseOptions(parseResult, options);

            return options;
        }
    }
}
