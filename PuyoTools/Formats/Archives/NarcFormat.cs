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
    internal class NarcFormat : IArchiveFormat
    {
        private NarcFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static NarcFormat Instance { get; } = new NarcFormat();

        public string Name => "NARC";

        public string FileExtension => ".narc";

        public ArchiveBase GetCodec() => new NarcArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => null;

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
