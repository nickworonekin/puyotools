using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Archives
{
    /// <inheritdoc/>
    internal partial class U8Format : IArchiveFormat
    {
        private U8Format() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static U8Format Instance { get; } = new U8Format();

        public string Name => "U8 (Wii ARChive)";

        public string FileExtension => ".arc";

        public ArchiveBase GetCodec() => new U8Archive();

        public bool Identify(Stream source, string filename) => U8Archive.Identify(source);
    }
}
