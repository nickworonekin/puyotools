using PuyoTools.App.Cli.Commands.Textures;
using PuyoTools.Core.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Textures
{
    /// <inheritdoc/>
    internal partial class GimFormat : ITextureFormat
    {
        public string CommandName => "gim";

        public TextureFormatEncodeCommand GetEncodeCommand() => new GimTextureEncodeCommand(this);
    }
}
