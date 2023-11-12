using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives
{
    public abstract class ArchiveWriter<T> : ArchiveWriter
        where T : ArchiveWriterEntry
    {
        protected List<T> _entries;

        protected ArchiveWriter(Stream destination) : base(destination)
        {
            _entries = new List<T>();
        }

        /// <summary>
        /// Creates a new archive entry.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name">The name of the entry to create in the archive.</param>
        /// <param name="leaveOpen"><see langword="true"/> to leave the stream open after the entry is written; otherwise, <see langword="false"/>.</param>
        /// <returns>The archive entry, or <see langword="null"/> if this entry cannot be added to the archive.</returns>
        protected abstract T? CreateEntry(Stream source, string? name = null, bool leaveOpen = false);

        protected virtual void WriteEntry(Stream destination, T entry)
        {
            OnEntryWriting(new ArchiveEntryWritingEventArgs(entry));

            entry.WriteTo(destination);

            OnEntryWritten(new ArchiveEntryWrittenEventArgs(entry));
        }

        public override void AddEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            if (!TryAddEntry(source, name, leaveOpen))
            {
                // TODO: Replace with a proper exception
                throw new Exception();
            }
        }

        public override bool TryAddEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            var entry = CreateEntry(source, name, leaveOpen);
            if (entry is null)
            {
                return false;
            }

            _entries.Add(entry);

            return true;
        }
    }
}
