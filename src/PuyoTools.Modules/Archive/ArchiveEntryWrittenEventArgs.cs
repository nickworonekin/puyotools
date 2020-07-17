using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Modules.Archive
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

        /// <summary>
        /// Gets the entry that was written to the archive.
        /// </summary>
        public ArchiveEntry Entry { get; }
    }
}
