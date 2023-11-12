using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives
{
    public abstract class ArchiveWriter
    {
        protected Stream _destination;

        protected ArchiveWriter(Stream destination)
        {
            _destination = destination;
        }

        /// <summary>
        /// Adds a new entry to the archive.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name">The name of the entry to create in the archive.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the stream open after the entry is written; otherwise, <see langword="false"/>.</param>
        public abstract void AddEntry(Stream source, string? name = null, bool leaveOpen = false);

        /// <summary>
        /// Tries to add a new entry to the archive, and returns a value that indicates whether the entry was added.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the stream open after the entry is written; otherwise, <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if the entry was added to the archive; otherwise, <see langword="false"/>.</returns>
        public abstract bool TryAddEntry(Stream source, string? name = null, bool leaveOpen = false);

        /// <summary>
        /// Adds a new entry from an existing file to the archive.
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="name">The name of the entry to create in the archive.</param>
        public void AddEntryFromFile(string sourceFileName, string? name = null)
            => AddEntry(new FileStream(sourceFileName, FileMode.Open, FileAccess.Read), name, true);

        /// <summary>
        /// Tries to add a new entry from an existing file to the archive, and returns a value that indicates whether the entry was added.
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="name">The name of the entry to create in the archive.</param>
        /// <returns><see langword="true"/> if the entry was added to the archive; otherwise, <see langword="false"/>.</returns>
        public bool TryAddEntryFromFile(string sourceFileName, string? name = null)
            => TryAddEntry(new FileStream(sourceFileName, FileMode.Open, FileAccess.Read), name, true);

        public abstract void Write(Stream destination);

        /// <summary>
        /// Occurs when an entry is being written to the archive.
        /// </summary>
        public event EventHandler<ArchiveEntryWritingEventArgs> EntryWriting;

        /// <summary>
        /// The entry written event is fired when an entry is being written to the archive.
        /// </summary>
        protected virtual void OnEntryWriting(ArchiveEntryWritingEventArgs e)
        {
            EntryWriting?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when an entry has been written to the archive.
        /// </summary>
        public event EventHandler<ArchiveEntryWrittenEventArgs> EntryWritten;

        /// <summary>
        /// The entry written event is fired when an entry has been written to the archive.
        /// </summary>
        protected virtual void OnEntryWritten(ArchiveEntryWrittenEventArgs e)
        {
            EntryWritten?.Invoke(this, e);
        }
    }
}
