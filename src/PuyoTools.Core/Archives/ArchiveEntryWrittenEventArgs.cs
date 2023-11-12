using PuyoTools.Archives;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Archives
{
    /// <summary>
    /// Provides data for the <see cref="ArchiveEntryWrittenEventHandler"/> event.
    /// </summary>
    public class ArchiveEntryWrittenEventArgs : EventArgs
    {
        public ArchiveEntryWrittenEventArgs(ArchiveEntry entry)
        {
            Entry = entry;
        }

        public ArchiveEntryWrittenEventArgs(ArchiveWriterEntry entry)
        {
            // TODO: Implement properties.
        }

        /// <summary>
        /// Gets the entry that was written to the archive.
        /// </summary>
        public ArchiveEntry Entry { get; }
    }
}
