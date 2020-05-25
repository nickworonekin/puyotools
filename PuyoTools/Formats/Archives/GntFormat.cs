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
    internal class GntFormat : IArchiveFormat
    {
        private GntFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static GntFormat Instance { get; } = new GntFormat();

        public string Name => "GNT";

        public string FileExtension => ".gnt";

        public ArchiveBase GetCodec() => new GntArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => null;

        public bool Identify(Stream source, string filename) => GntArchive.Identify(source);
    }
}
