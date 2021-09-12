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
    internal class U8Format : IArchiveFormat
    {
        private U8Format() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static U8Format Instance { get; } = new U8Format();

        public string Name => "U8 (Wii ARChive)";

        public string FileExtension => ".arc";

        public ArchiveBase GetCodec() => new U8Archive();

        public ModuleSettingsControl GetModuleSettingsControl() => null;

        public bool Identify(Stream source, string filename) => U8Archive.Identify(source);
    }
}
