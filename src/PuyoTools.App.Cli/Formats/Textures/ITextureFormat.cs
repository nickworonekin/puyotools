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
    internal partial interface ITextureFormat
    {
        /// <summary>
        /// Gets the command name of this texture format.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Gets the encode command for this texture format.
        /// </summary>
        /// <returns>The encode command.</returns>
        TextureFormatEncodeCommand GetEncodeCommand();
    }
}
