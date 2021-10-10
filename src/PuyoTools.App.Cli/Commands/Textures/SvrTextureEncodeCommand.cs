using PuyoTools.App.Formats.Textures;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.Core.Textures.Svr;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class SvrTextureEncodeCommand : TextureFormatEncodeCommand
    {
        public SvrTextureEncodeCommand(SvrFormat format)
            : base(format)
        {
            AddOption(new Option<SvrPixelFormat>("--pixel-format", "Set the pixel format")
            {
                IsRequired = true,
            });
            AddOption(new Option<SvrDataFormat>("--data-format", "Set the data format")
            {
                IsRequired = true,
            });
            AddOption(new Option<uint?>("--global-index", "Adds the GBIX header, optionally with a global index.")
            {
                Arity = ArgumentArity.ZeroOrOne,
            });

            Handler = CommandHandler.Create<SvrTextureEncodeOptions, IConsole>(Execute);
        }

        private void Execute(SvrTextureEncodeOptions options, IConsole console) => base.Execute(options, console);
    }
}
