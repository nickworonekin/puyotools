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
    internal class PvmFormat : IArchiveFormat
    {
        private PvmFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static PvmFormat Instance { get; } = new PvmFormat();

        public string Name => "PVM";

        public string FileExtension => ".pvm";

        public ArchiveBase GetCodec() => new PvmArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => new PvmWriterSettings();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
