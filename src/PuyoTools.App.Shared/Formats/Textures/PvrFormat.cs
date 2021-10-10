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
    internal partial class PvrFormat : ITextureFormat
    {
        private PvrFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static PvrFormat Instance { get; } = new PvrFormat();

        public string Name => "PVR";

        public string FileExtension => ".pvr";

        public string PaletteFileExtension => ".pvp";

        public TextureBase GetCodec() => new PvrTexture();

        public bool Identify(Stream source, string filename) => PvrTexture.Identify(source);
    }
}
