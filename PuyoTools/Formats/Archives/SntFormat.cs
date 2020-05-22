using PuyoTools.Formats.Archives.WriterSettings;
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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class SntFormat : IArchiveFormat
    {
        private SntFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static SntFormat Instance { get; } = new SntFormat();

        public string Name => "SNT";

        public string FileExtension => ".Snt";

        public ArchiveBase GetCodec() => new SntArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => new SntWriterSettings();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
