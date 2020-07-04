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
    internal class SvmFormat : IArchiveFormat
    {
        private SvmFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static SvmFormat Instance { get; } = new SvmFormat();

        public string Name => "SVM";

        public string FileExtension => ".svm";

        public ArchiveBase GetCodec() => new SvmArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => new PvmWriterSettings();

        public bool Identify(Stream source, string filename) => SvmArchive.Identify(source);
    }
}
