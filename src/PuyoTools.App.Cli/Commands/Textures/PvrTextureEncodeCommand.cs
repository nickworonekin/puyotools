using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.App.Cli.Commands.Archives;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures.Pvr;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class PvrTextureEncodeCommand : TextureFormatEncodeCommand
    {
        private readonly Option<PvrPixelFormat> _pixelFormatOption;
        private readonly Option<PvrDataFormat> _dataFormatOption;
        private readonly Option<uint?> _globalIndexOption;
        private readonly Option<bool> _rleCompressionOption;
        private readonly Option<bool> _ditherOption;

        public PvrTextureEncodeCommand(PvrFormat format)
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

            _rleCompressionOption = new("--rle-compression")
            {
                Description = "RLE compression (PVZ) for Puyo Puyo Fever.",
            };
            Add(_rleCompressionOption);

            _ditherOption = new("--dither")
            {
                Description = "Use dithering when creating palette-based textures.",
            };
            Add(_ditherOption);
        }

        protected override TextureFormatEncodeOptions CreateOptions(ParseResult parseResult)
        {
            PvrTextureEncodeOptions options = new()
            {
                PixelFormat = parseResult.GetRequiredValue(_pixelFormatOption),
                DataFormat = parseResult.GetRequiredValue(_dataFormatOption),
                GlobalIndex = parseResult.GetValue(_globalIndexOption),
                RleCompression = parseResult.GetValue(_rleCompressionOption),
                Dither = parseResult.GetValue(_ditherOption),
            };

            SetBaseOptions(parseResult, options);

            return options;
        }
    }
}
