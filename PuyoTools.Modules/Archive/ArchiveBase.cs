using System;
using System.Collections.Generic;
using System.IO;

namespace PuyoTools.Modules.Archive
{
    public abstract class ArchiveBase : ModuleBase
    {
        /// <summary>
        /// The primary file extension for this archive format.
        /// </summary>
        public abstract string FileExtension { get; }

        #region Open Methods
        /// <summary>
        /// Open an archive from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>An ArchiveReader object.</returns>
        /// <remarks>You must keep the stream open for the duration of the ArchiveReader.</remarks>
        public abstract ArchiveReader Open(Stream source);

        /// <summary>
        /// Open an archive from part of a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>An ArchiveReader object.</returns>
        /// <remarks>You must keep the stream open for the duration of the ArchiveReader.</remarks>
        public ArchiveReader Open(Stream source, int length)
        {
            return Open(new StreamView(source, length));
        }

        /// <summary>
        /// Open an archive from a file.
        /// </summary>
        /// <param name="path">File to open.</param>
        /// <returns>An ArchiveReader object.</returns>
        public ArchiveReader Open(string path)
        {
            return Open(File.OpenRead(path));
        }

        /// <summary>
        /// Open an archive from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>An ArchiveReader object.</returns>
        public ArchiveReader Open(byte[] source)
        {
            return Open(source, 0, source.Length);
        }

        /// <summary>
        /// Open an archive from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">Offset of the data in the source array.</param>
        /// <param name="length">Length of the data in the source array.</param>
        /// <returns>An ArchiveReader object.</returns>
        public ArchiveReader Open(byte[] source, int offset, int length)
        {
            MemoryStream sourceStream = new MemoryStream();
            sourceStream.Write(source, 0, length);
            sourceStream.Position = 0;

            return Open(sourceStream);
        }
        #endregion

        #region Create Methods
        /// <summary>
        /// Create an archive.
        /// </summary>
        /// <param name="destination">The stream to write to.</param>
        /// <returns>An ArchiveWriter object.</returns>
        public abstract ArchiveWriter Create(Stream destination);
        #endregion
    }

    public abstract class ArchiveReader : IModule
    {
        /// <summary>
        /// The starting offset of the archive within the stream.
        /// </summary>
        protected long startOffset;

        protected Stream archiveData;
        protected ArchiveEntryCollection entries;

        public ArchiveReader(Stream source)
        {
            archiveData = source;
            startOffset = source.Position;
        }

        /// <summary>
        /// Gets the collection of entries that are currently in the archive.
        /// </summary>
        public ArchiveEntryCollection Entries
        {
            get { return entries; }
        }

        /// <summary>
        /// Retrieves the specified entry in the archive.
        /// </summary>
        /// <param name="index">Index of the entry within the archive.</param>
        /// <returns>An ArchiveEntry.</returns>
        public ArchiveEntry GetEntry(int index)
        {
            return entries[index];
        }

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
        /// Opens the specified entry in the archive.
        /// </summary>
        /// <param name="index">Index of the entry within the archive.</param>
        /// <returns>The stream that represents the contents of the entry.</returns>
        public Stream OpenEntry(int index)
        {
            return OpenEntry(entries[index]);
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
                PTStream.CopyTo(source, destination);
            }
        }

        /// <summary>
        /// Extracts the specified entry to a file.
        /// </summary>
        /// <param name="index">Index of the entry within the archive.</param>
        /// <param name="path">The path to extract the file to.</param>
        public void ExtractToFile(int index, string path)
        {
            ExtractToFile(entries[index], path);
        }

        /// <summary>
        /// Adds the specified entry to the ArchiveEntryCollection associated with this archive.
        /// </summary>
        /// <param name="entry">An archive entry.</param>
        protected void AddEntry(ArchiveEntry entry)
        {
            entries.Add(entry);
        }
    }

    public abstract class ArchiveWriter : IModule
    {
        protected Stream destination;

        protected ArchiveEntryCollection entries;

        public ArchiveWriter() { }

        public ArchiveWriter(Stream destination)
        {
            this.destination = destination;

            entries = new ArchiveEntryCollection(this);
        }

        /// <summary>
        /// Gets the collection of entries that are currently in the archive.
        /// </summary>
        public ArchiveEntryCollection Entries
        {
            get { return entries; }
        }

        public abstract void Flush();

        /// <summary>
        /// Creates an entry that has the specified data entry name in the archive.
        /// </summary>
        /// <param name="source">The data to be added to the archive.</param>
        /// <param name="entryName">The name of the entry to be created.</param>
        /// <remarks>
        /// The file may be rejected from the archive. In this case, a CannotAddFileToArchiveException will be thrown.
        /// </remarks>
        public virtual void CreateEntry(Stream source, string entryName)
        {
            entries.Add(new ArchiveEntry(this, source, entryName));
        }

        /// <summary>
        /// Creates an entry with no file name that has the specified data entry name in the archive.
        /// </summary>
        /// <param name="source">The data to be added to the archive.</param>
        /// <remarks>
        /// The file may be rejected from the archive. In this case, a CannotAddFileToArchiveException will be thrown.
        /// </remarks>
        public void CreateEntry(Stream source)
        {
            CreateEntry(source, String.Empty);
        }

        /// <summary>
        /// Adds an existing file to the archive.
        /// </summary>
        /// <param name="path">The path of the file to be added.</param>
        /// <param name="entryName">The name of the entry to be created.</param>
        /// <remarks>
        /// The file may be rejected from the archive. In this case, a CannotAddFileToArchiveException will be thrown.
        /// </remarks>
        public void CreateEntryFromFile(string path, string entryName)
        {
            CreateEntry(File.OpenRead(path), entryName);
        }

        /// <summary>
        /// Adds an existing file to the archive.
        /// </summary>
        /// <param name="path">The path of the file to be added.</param>
        /// <remarks>
        /// The file may be rejected from the archive. In this case, a CannotAddFileToArchiveException will be thrown.
        /// </remarks>
        public void CreateEntryFromFile(string path)
        {
            CreateEntry(File.OpenRead(path), Path.GetFileName(path));
        }

        // File added event
        public delegate void FileAddedHandler(object sender, EventArgs e);
        public event EventHandler FileAdded;
        protected virtual void OnFileAdded(EventArgs e)
        {
            EventHandler handler = FileAdded;
            if (handler != null)
            {
                FileAdded(this, e);
            }
        }
    }

    public class CannotAddFileToArchiveException : Exception { }
}