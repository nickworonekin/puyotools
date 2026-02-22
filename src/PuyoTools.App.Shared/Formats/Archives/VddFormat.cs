using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Vdd;
using PuyoTools.Core.Archives;

namespace PuyoTools.App.Formats.Archives
{
    /// <inheritdoc/>
    internal partial class VddFormat : IArchiveFormat
    {
        private VddFormat() { }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        internal static VddFormat Instance { get; } = new VddFormat();

        public string Name => "VDD";

        public string FileExtension => ".vdd";

        public ArchiveBase GetCodec() => new GntArchive();

        public ArchiveReader CreateReader(Stream source) => new VddReader(source);

        public ArchiveWriter CreateWriter(Stream destination) => new VddWriter(destination);

        public bool Identify(Stream source, string filename) => Path.GetExtension(filename).Equals(FileExtension, StringComparison.OrdinalIgnoreCase) && VddReader.IsFormat(source);
    }
}
