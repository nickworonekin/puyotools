using PuyoTools.GUI;
using PuyoTools.Modules;
using PuyoTools.Modules.Archive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Formats.Archives
{
    /// <inheritdoc/>
    internal class SpkFormat : IArchiveFormat
    {
        private SpkFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static SpkFormat Instance { get; } = new SpkFormat();

        public string Name => "SPK (Puyo Puyo Fever 2)";

        public string FileExtension => ".spk";

        public ArchiveBase GetCodec() => new SpkArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => null;

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
