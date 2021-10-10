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
        private SvrFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static SvrFormat Instance { get; } = new SvrFormat();

        public string Name => "SVR";

        public string FileExtension => ".svr";

        public string PaletteFileExtension => ".svp";

        public TextureBase GetCodec() => new SvrTexture();

        public bool Identify(Stream source, string filename) => SvrTexture.Identify(source);
    }
}
