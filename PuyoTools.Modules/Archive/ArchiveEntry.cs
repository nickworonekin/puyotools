using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace PuyoTools.Modules.Archive
{
    public class ArchiveEntry
    {
        private Mode mode; // Read/Write mode for this archive entry.
        private ArchiveReader archiveReader; // The archive reader associated with this entry
        private ArchiveWriter archiveWriter; // The archive writer associated with this entry
        private long offset; // The offset of the entry within the archive
        private int length; // The size of the entry within the archive
        private string name; // The name of the entry within the archive
        private Stream data; // Stream data for the entry
        private string path; // The file path for the entry

        /// <summary>
        /// Creates a new instance of ArchiveEntry
        /// </summary>
        /// <param name="archive">The archive this entry belongs to.</param>
        /// <param name="offset">The offset of the entry within the archive.</param>
        /// <param name="length">The size of the entry within the archive.</param>
        /// <param name="name">The file name of the entry.</param>
        internal ArchiveEntry(ArchiveReader archiveReader, long offset, int length, string name)
        {
            mode = Mode.Read;
            this.archiveReader = archiveReader;

            this.offset = offset;
            this.length = length;
             
            this.name = name;
            this.path = String.Empty;
        }

        /// <summary>
        /// Creates an entry that has the specified data and entry name in the archive.
        /// </summary>
        /// <param name="source">The data to be added to the archive.</param>
        /// <param name="name">A path that specifies the name of the entry to be created.</param>
        internal ArchiveEntry(ArchiveWriter archiveWriter, Stream source, string name)
        {
            mode = Mode.Write;
            this.archiveWriter = archiveWriter;

            data = new StreamView(source);
            this.offset = 0;
            this.length = (int)data.Length;
            this.name = name;

            if (source is FileStream)
            {
                this.path = ((FileStream)source).Name;
            }
            else
            {
                this.path = String.Empty;
            }
        }

        /// <summary>
        /// Returns the ArchiveReader that contains this entry.
        /// </summary>
        public ArchiveReader ArchiveReader
        {
            get
            {
                if (mode != Mode.Read)
                {
                    throw new NotSupportedException();
                }

                return archiveReader;
            }
        }

        /// <summary>
        /// Returns the ArchiveWriter that contains this entry.
        /// </summary>
        public ArchiveWriter ArchiveWriter
        {
            get
            {
                if (mode != Mode.Write)
                {
                    throw new NotSupportedException();
                }

                return archiveWriter;
            }
        }

        /// <summary>
        /// Gets the zero-based index of the item within the ListViewItemCollection.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets the offset of the entry within the archive.
        /// </summary>
        internal long Offset
        {
            get { return offset; }
        }

        /// <summary>
        /// Gets the size of the entry as stored in the archive.
        /// </summary>
        /// <remarks>
        /// This may not be the length of the stream you get when you call Open.
        /// You should use the size of the stream instead when reading the stream.
        /// </remarks>
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        /// The file name of the entry.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The file path for the entry (or blank if it is not known).
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Opens the entry from the archive for reading only.
        /// </summary>
        /// <returns>The stream that represents the contents of the entry.</returns>
        public Stream Open()
        {
            if (mode == Mode.Write)
            {
                data.Position = offset;
                return data;
            }

            return archiveReader.OpenEntry(this);
        }

        /// <summary>
        /// Indicates if this archive entry is for an ArchiveReader (Mode.Read) or an ArchiveWriter (Mode.Write).
        /// </summary>
        private enum Mode
        {
            Read,
            Write,
        }
    }

    /// <summary>
    /// A read-only collection of archive entries.
    /// </summary>
    public class ArchiveEntryCollection
    {
        private ArchiveReader ownerReader; // Archive reader owner
        private ArchiveWriter ownerWriter; // Archive writer owner

        private List<ArchiveEntry> entries; // Archive entries

        /// <summary>
        /// Initalizes a new instance of the ArchiveEntryCollection class.
        /// </summary>
        /// <param name="owner">The ArchiveReader that owns this collection.</param>
        /// <param name="numEntries">The total number of entries in the archive.</param>
        public ArchiveEntryCollection(ArchiveReader owner, int numEntries)
        {
            ownerReader = owner;

            entries = new List<ArchiveEntry>(numEntries);
        }

        /// <summary>
        /// Initalizes a new instance of the ArchiveEntryCollection class.
        /// </summary>
        /// <param name="owner">The ArchiveWriter that owns this collection.</param>
        internal ArchiveEntryCollection(ArchiveWriter owner)
        {
            ownerWriter = owner;

            entries = new List<ArchiveEntry>();
        }

        /// <summary>
        /// Gets the number of elements contained in the ArchiveEntryCollection.
        /// </summary>
        public int Count
        {
            get { return entries.Count; }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public ArchiveEntry this[int index]
        {
            get { return entries[index]; }
        }

        /// <summary>
        /// Adds an item to the ArchiveEntryCollection.
        /// </summary>
        /// <param name="item"></param>
        internal void Add(ArchiveEntry item)
        {
            entries.Add(item);
            entries[entries.Count - 1].Index = entries.Count - 1;
        }

        /// <summary>
        /// Adds an item to the ArchiveEntryCollection.
        /// </summary>
        /// <param name="offset">The offset of the entry within the archive.</param>
        /// <param name="length">The size of the entry within the archive.</param>
        /// <param name="name">The file name of the entry.</param>
        internal void Add(long offset, int length, string name)
        {
            entries.Add(new ArchiveEntry(ownerReader, offset, length, name));
            entries[entries.Count - 1].Index = entries.Count - 1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ArchiveEntry> GetEnumerator()
        {
            foreach (ArchiveEntry entry in entries)
            {
                yield return entry;
            }
        }
    }
}