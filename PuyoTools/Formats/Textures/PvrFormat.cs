using PuyoTools.GUI;
using PuyoTools.Modules.Texture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Formats.Textures
{
    /// <inheritdoc/>
    internal class PvrFormat : ITextureFormat
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

        public ModuleSettingsControl GetModuleSettingsControl() => new PvrWriterSettings();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
