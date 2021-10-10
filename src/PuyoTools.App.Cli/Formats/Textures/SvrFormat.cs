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
    internal partial class SvrFormat : ITextureFormat
    {
        public string CommandName => "svr";

        public TextureFormatEncodeCommand GetEncodeCommand() => new SvrTextureEncodeCommand(this);
    }
}
