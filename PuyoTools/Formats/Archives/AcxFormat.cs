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
    /// <inheritdoc/>
    internal class AcxFormat : IArchiveFormat
    {
        private AcxFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static AcxFormat Instance { get; } = new AcxFormat();

        public string Name => "ACX";

        public string FileExtension => ".acx";

        public ArchiveBase GetCodec() => new AcxArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => new AcxWriterSettings();

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
