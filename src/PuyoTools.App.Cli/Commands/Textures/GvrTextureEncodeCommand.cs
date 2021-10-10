using PuyoTools.App.Formats.Textures;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
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
            AddOption(new Option<GvrPixelFormat>("--palette-format", "Set the palette format"));
            AddOption(new Option<GvrDataFormat>("--data-format", "Set the data format")
            {
                IsRequired = true,
            });
            AddOption(new Option<uint?>("--global-index", "Adds the GBIX header, optionally with a global index.")
            {
                Arity = ArgumentArity.ZeroOrOne,
            });
            AddOption(new Option("--gcix", "Use GCIX global index header."));

            Handler = CommandHandler.Create<GvrTextureEncodeOptions, IConsole>(Execute);
        }

        private void Execute(GvrTextureEncodeOptions options, IConsole console) => base.Execute(options, console);
    }
}
