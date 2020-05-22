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
    internal class MrgFormat : IArchiveFormat
    {
        private MrgFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static MrgFormat Instance { get; } = new MrgFormat();

        public string Name => "MRG (Puyo Puyo Fever 2)";

        public string FileExtension => ".mrg";

        public ArchiveBase GetCodec() => new MrgArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => null;

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
