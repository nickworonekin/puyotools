using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace PuyoTools.Core.Archives
{
    public class ArchiveEntry
    {
        private Mode mode; // Read/Write mode for this archive entry.
        protected LegacyArchiveReader archiveReader; // The archive reader associated with this entry
        private LegacyArchiveWriter archiveWriter; // The archive writer associated with this entry
        protected long offset; // The offset of the entry within the archive
        protected int length; // The size of the entry within the archive
        private string name; // The name of the entry within the archive
        private Stream data; // Stream data for the entry
        private string path; // The file path for the entry

        /// <summary>
        /// Creates a new instance of ArchiveEntry
        /// </summary>
        /// <param name="archiveReader">The <see cref="ArchiveReader"/> this entry belongs to.</param>
        /// <param name="offset">The offset of the entry within the archive.</param>
        /// <param name="length">The size of the entry within the archive.</param>
        /// <param name="name">The file name of the entry.</param>
        internal ArchiveEntry(LegacyArchiveReader archiveReader, long offset, int length, string name)
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
        /// <param name="archiveWriter">The <see cref="ArchiveWriter"/> this entry belongs to.</param>
        /// <param name="source">The data to be added to the archive.</param>
        /// <param name="name">A path that specifies the name of the entry to be created.</param>
        internal ArchiveEntry(LegacyArchiveWriter archiveWriter, Stream source, string name)
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
        public LegacyArchiveReader ArchiveReader
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
        public LegacyArchiveWriter ArchiveWriter
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
        /// Gets the zero-based index of the item within the <see cref="ArchiveEntry"/> collection.
        /// </summary>
        /// <remarks>This value is only set when <see cref="ArchiveReader"/> is an instance of <see cref="GvmArchive"/>, <see cref="PvmArchive"/>, or <see cref="SvmArchive"/>.</remarks>
        //public int Index { get; internal set; }

        /// <summary>
        /// Gets the offset of the entry within the archive.
        /// </summary>
        /*internal long Offset
        {
            get { return offset; }
        }*/

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

        public string FullName => name;

        /// <summary>
        /// The file name of the entry.
        /// </summary>
        public string Name => System.IO.Path.GetFileName(name);

        /// <summary>
        /// When this entry belongs to an <see cref="Archives.LegacyArchiveWriter"/>, gets the absolute path of the file used to create this entry.
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Opens the entry from the archive for reading only.
        /// </summary>
        /// <returns>The stream that represents the contents of the entry.</returns>
        public virtual Stream Open()
        {
            if (mode == Mode.Write)
            {
                data.Position = offset;
                return data;
            }

            //return archiveReader.OpenEntry(this);
            return new StreamView(archiveReader.ArchiveStream, offset, length);
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
}