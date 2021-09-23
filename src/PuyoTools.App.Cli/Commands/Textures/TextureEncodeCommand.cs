using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Cli.Commands.Textures
{
    class TextureEncodeCommand : Command
    {
        public TextureEncodeCommand()
            : base("encode", "Encode textures")
        {
            // Add commands for all texture formats that can be used to encode textures.
            foreach (var format in TextureFactory.EncoderFormats)
            {
                AddCommand(format.GetEncodeCommand());
            }
        }
    }
}
