using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives
{
    public abstract class ArchiveReader
    {
        /// <summary>
        /// Gets the collection of entries that are currently in the archive.
        /// </summary>
        public abstract IReadOnlyList<ArchiveReaderEntry> Entries { get; }
    }
}
