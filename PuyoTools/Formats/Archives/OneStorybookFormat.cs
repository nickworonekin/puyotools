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
    internal class OneStorybookFormat : IArchiveFormat
    {
        private OneStorybookFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static OneStorybookFormat Instance { get; } = new OneStorybookFormat();

        public string Name => "ONE (Sonic Storybook series)";

        public string FileExtension => ".one";

        public ArchiveBase GetCodec() => new OneStorybookArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => null;

        public bool Identify(Stream source, string filename) => OneStorybookArchive.Identify(source);
    }
}
