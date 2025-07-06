using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures.Pvr;
using PuyoTools.Core.Textures.Svr;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class SvrTextureEncodeCommand : TextureFormatEncodeCommand
    {
        private readonly Option<SvrPixelFormat> _pixelFormatOption;
        private readonly Option<SvrDataFormat> _dataFormatOption;
        private readonly Option<uint?> _globalIndexOption;
        private readonly Option<bool> _ditherOption;

        public SvrTextureEncodeCommand(SvrFormat format)
            : base(format)
        {
            _pixelFormatOption = new("--pixel-format")
            {
                Description = "Set the pixel format",
                Required = true,
            };
            Add(_pixelFormatOption);

            _dataFormatOption = new("--data-format")
            {
                Description = "Set the data format",
                Required = true,
            };
            Add(_dataFormatOption);

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

            _ditherOption = new("--dither")
            {
                Description = "Use dithering when creating palette-based textures.",
            };
            Add(_ditherOption);
        }

        protected override TextureFormatEncodeOptions CreateOptions(ParseResult parseResult)
        {
            SvrTextureEncodeOptions options = new()
            {
                PixelFormat = parseResult.GetRequiredValue(_pixelFormatOption),
                DataFormat = parseResult.GetRequiredValue(_dataFormatOption),
                GlobalIndex = parseResult.GetValue(_globalIndexOption),
                Dither = parseResult.GetValue(_ditherOption),
            };

            SetBaseOptions(parseResult, options);

            return options;
        }
    }
}
