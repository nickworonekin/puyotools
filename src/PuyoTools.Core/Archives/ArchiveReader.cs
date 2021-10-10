using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Archives
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
        /// Extracts the specified entry to a file.
        /// </summary>
        /// <param name="entry">An archive entry.</param>
        /// <param name="path">The path to extract the file to.</param>
        public void ExtractToFile(ArchiveEntry entry, string path)
        {
            using (Stream source = entry.Open()/*OpenEntry(entry)*/, destination = File.Create(path))
            {
                source.CopyTo(destination);
            }
        }

        internal Stream ArchiveStream => archiveData;
    }
}