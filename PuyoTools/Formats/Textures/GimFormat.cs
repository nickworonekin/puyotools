using PuyoTools.Formats.Textures.WriterSettings;
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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class GimFormat : ITextureFormat
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

        public ModuleSettingsControl GetModuleSettingsControl() => new GimWriterSettings();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
