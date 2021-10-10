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
    internal partial class GvrFormat : ITextureFormat
    {
        private GvrFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static GvrFormat Instance { get; } = new GvrFormat();

        public string Name => "GVR";

        public string FileExtension => ".gvr";

        public string PaletteFileExtension => ".gvp";

        public TextureBase GetCodec() => new GvrTexture();

        public bool Identify(Stream source, string filename) => GvrTexture.Identify(source);
    }
}
