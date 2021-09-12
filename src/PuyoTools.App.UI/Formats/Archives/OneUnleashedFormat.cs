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
    internal class OneUnleashedFormat : IArchiveFormat
    {
        private OneUnleashedFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static OneUnleashedFormat Instance { get; } = new OneUnleashedFormat();

        public string Name => "ONE (Sonic Unleashed)";

        public string FileExtension => ".one";

        public ArchiveBase GetCodec() => new OneStorybookArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => null;

        public bool Identify(Stream source, string filename) => OneUnleashedArchive.Identify(source);
    }
}
