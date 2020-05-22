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
    internal class TxdStorybookFormat : IArchiveFormat
    {
        private TxdStorybookFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static TxdStorybookFormat Instance { get; } = new TxdStorybookFormat();

        public string Name => "TXD (Sonic Storybook series)";

        public string FileExtension => ".txd";

        public ArchiveBase GetCodec() => new TxdStorybookArchive();

        public ModuleSettingsControl GetModuleSettingsControl() => null;

        public bool Identify(Stream source, string filename) => GetCodec().Is(source, filename);
    }
}
