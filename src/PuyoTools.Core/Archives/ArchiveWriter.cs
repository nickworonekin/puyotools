using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public abstract class ArchiveWriter : IDisposable, IModule
    {
        protected Stream destination;

        protected List<ArchiveEntry> entries;

        private bool disposed;

        public ArchiveWriter() { }

        public ArchiveWriter(Stream destination)
        {
            this.destination = destination;

            entries = new List<ArchiveEntry>();
        }

        /// <summary>
        /// Gets the collection of entries that are currently in the archive.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="ArchiveWriter"/> has already been closed.</exception>
        public ReadOnlyCollection<ArchiveEntry> Entries
        {
            get
            {
                ThrowIfDisposed();
                return entries.AsReadOnly();
            }
        }

        protected abstract void WriteFile();

        /// <summary>
        /// Creates an entry that has the specified data entry name in the archive.
        /// </summary>
        /// <param name="source">The data to be added to the archive.</param>
        /// <param name="entryName">The name of the entry to be created.</param>
        /// <remarks>If the file cannot be added to the archive (for example, an archive may only accept files of a certain type), <see cref="FileRejectedException"/> will be thrown.</remarks>
        /// <returns>The entry that was created in the archive.</returns>
        /// <exception cref="FileRejectedException">The file could not be added to the archive.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ArchiveWriter"/> has already been closed.</exception>
        public virtual ArchiveEntry CreateEntry(Stream source, string entryName)
        {
            ThrowIfDisposed();

            var entry = new ArchiveEntry(this, source, entryName);
            entries.Add(entry);

            return entry;
        }

        /// <summary>
        /// Creates an entry with no file name that has the specified data entry name in the archive.
        /// </summary>
        /// <param name="source">The data to be added to the archive.</param>
        /// <remarks>
        /// The file may be rejected from the archive. In this case, a CannotAddFileToArchiveException will be thrown.
        /// </remarks>
        public ArchiveEntry CreateEntry(Stream source)
        {
            return CreateEntry(source, String.Empty);
        }

        /// <summary>
        /// Adds an existing file to the archive.
        /// </summary>
        /// <param name="path">The path of the file to be added.</param>
        /// <param name="entryName">The name of the entry to be created.</param>
        /// <remarks>
        /// The file may be rejected from the archive. In this case, a CannotAddFileToArchiveException will be thrown.
        /// </remarks>
        public ArchiveEntry CreateEntryFromFile(string path, string entryName)
        {
            return CreateEntry(File.OpenRead(path), entryName);
        }

        /// <summary>
        /// Adds an existing file to the archive.
        /// </summary>
        /// <param name="path">The path of the file to be added.</param>
        /// <remarks>
        /// The file may be rejected from the archive. In this case, a CannotAddFileToArchiveException will be thrown.
        /// </remarks>
        public ArchiveEntry CreateEntryFromFile(string path)
        {
            return CreateEntry(File.OpenRead(path), Path.GetFileName(path));
        }

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

        protected void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                try
                {
                    WriteFile();
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        /// <summary>
        /// Releases the resources used by the current instance of the <see cref="ArchiveWriter"/> class.
        /// </summary>
        /// <remarks>This method finishes writing the archive and releases all resources used by the <see cref="ArchiveWriter"/> object.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}