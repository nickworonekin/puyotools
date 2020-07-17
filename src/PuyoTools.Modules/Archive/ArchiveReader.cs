using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace PuyoTools.Modules.Archive
{
    public abstract class ArchiveReader : IModule
    {
        /// <summary>
        /// The starting offset of the archive within the stream.
        /// </summary>
        protected long startOffset;

        protected Stream archiveData;
        protected List<ArchiveEntry> entries;

        public ArchiveReader(Stream source)
        {
            archiveData = source;
            startOffset = source.Position;
        }

        /// <summary>
        /// Gets the collection of entries that are currently in the archive.
        /// </summary>
        public ReadOnlyCollection<ArchiveEntry> Entries => entries.AsReadOnly();

        /// <summary>
        /// Opens the specified entry in the archive.
        /// </summary>
        /// <param name="entry">An archive entry.</param>
        /// <returns>The stream that represents the contents of the entry.</returns>
        public virtual Stream OpenEntry(ArchiveEntry entry)
        {
            if (entry == null || entry.ArchiveReader != this)
            {
                throw new ArgumentException("entry");
            }

            return new StreamView(archiveData, entry.Offset, entry.Length);
        }

        /// <summary>
        /// Extracts the specified entry to a file.
        /// </summary>
        /// <param name="entry">An archive entry.</param>
        /// <param name="path">The path to extract the file to.</param>
        public void ExtractToFile(ArchiveEntry entry, string path)
        {
            using (Stream source = OpenEntry(entry), destination = File.Create(path))
            {
                source.CopyTo(destination);
            }
        }
    }
}