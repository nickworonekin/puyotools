using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures.Gim;
using PuyoTools.Core.Textures.Gvr;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class GimTextureEncodeCommand : TextureFormatEncodeCommand
    {
        private readonly Option<GimPaletteFormat> _paletteFormatOption;
        private readonly Option<GimPixelFormat> _pixelFormatOption;
        private readonly Option<bool> _metadataOption;
        private readonly Option<bool> _swizzleOption;
        private readonly Option<bool> _ditherOption;

        public GimTextureEncodeCommand(GimFormat format)
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

            _metadataOption = new("--metadata")
            {
                Description = "Include metadata",
            };
            Add(_metadataOption);

            _swizzleOption = new("--swizzle")
            {
                Description = "Use texture swizzling (for PSP only).",
            };
            Add(_swizzleOption);

            _ditherOption = new("--dither")
            {
                Description = "Use dithering when creating palette-based textures.",
            };
            Add(_ditherOption);
        }

        protected override TextureFormatEncodeOptions CreateOptions(ParseResult parseResult)
        {
            GimTextureEncodeOptions options = new()
            {
                PaletteFormat = parseResult.GetValue(_paletteFormatOption),
                PixelFormat = parseResult.GetRequiredValue(_pixelFormatOption),
                Metadata = parseResult.GetValue(_metadataOption),
                Swizzle = parseResult.GetValue(_swizzleOption),
                Dither = parseResult.GetValue(_ditherOption),
            };

            SetBaseOptions(parseResult, options);

            return options;
        }
    }
}
