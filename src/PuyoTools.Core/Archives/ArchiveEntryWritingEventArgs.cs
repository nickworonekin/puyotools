using PuyoTools.Archives;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Archives
{
    /// <summary>
    /// Provides data for the <see cref="ArchiveEntryWritingEventHandler"/> event.
    /// </summary>
    public class ArchiveEntryWritingEventArgs : EventArgs
    {
        public ArchiveEntryWritingEventArgs(ArchiveEntry entry)
        {
            Entry = entry;
        }

        public ArchiveEntryWritingEventArgs(ArchiveWriterEntry entry)
        {
            // TODO: Implement properties.
        }

        /// <summary>
        /// Gets the entry that is being written to the archive.
        /// </summary>
        public ArchiveEntry Entry { get; }
    }
}