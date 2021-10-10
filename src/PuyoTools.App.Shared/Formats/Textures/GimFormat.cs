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
        private GimFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static GimFormat Instance { get; } = new GimFormat();

        public string Name => "GIM";

        public string FileExtension => ".gim";

        public string PaletteFileExtension => string.Empty;

        public TextureBase GetCodec() => new GimTexture();

        public bool Identify(Stream source, string filename) => GimTexture.Identify(source);
    }
}
