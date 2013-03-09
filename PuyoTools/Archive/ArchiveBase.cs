using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools2.Archive
{
    public abstract class ArchiveBase
    {
        public abstract ArchiveReader Open(Stream source, int length);
        public abstract ArchiveWriter Create(Stream destination, ArchiveWriterSettings settings);
        public abstract bool Is(Stream source, int length, string fname);

        public ArchiveWriter Create(Stream destination)
        {
            return Create(destination, new ArchiveWriterSettings());
        }
    }

    public abstract class ArchiveReader
    {
        public ArchiveEntry[] Files;

        /// <summary>
        /// Returns information about a file at the specified index in the archive.
        /// </summary>
        /// <param name="index">Index of the file.</param>
        /// <returns>An ArchiveEntry containing the stream, offset in the stream, length, and the filename of the file.</returns>
        public virtual ArchiveEntry GetFile(int index)
        {
            // For most archives, GetFile will just return a reference to Files[index].
            // As such, it's much better to just impliment this as a virtual method
            // and only override it if necessary.

            // Make sure index is not out of bounds
            if (index < 0 || index > Files.Length)
                throw new IndexOutOfRangeException();

            return Files[index];
        }
    }

    public abstract class ArchiveWriter
    {
        protected Stream destination;
        protected List<ArchiveEntry> files;
        protected ArchiveWriterSettings settings;

        public abstract void Flush();

        protected void Initalize(Stream destination, ArchiveWriterSettings settings)
        {
            files = new List<ArchiveEntry>();
            this.destination = destination;
            this.settings = settings;
        }

        public void AddFile(Stream source, string fname)
        {
            AddFile(source, (int)(source.Length - source.Position), fname);
        }

        public void AddFile(Stream source, string fname, DateTime date)
        {
            AddFile(source, (int)(source.Length - source.Position), fname, date);
        }

        public void AddFile(Stream source, int length, string fname)
        {
            // All we're going to do is add the file to the entry list
            // The magic happens once Flush is called.
            files.Add(new ArchiveEntry(source, source.Position, length, fname));
        }

        public void AddFile(Stream source, int length, string fname, DateTime date)
        {
            // All we're going to do is add the file to the entry list
            // The magic happens once Flush is called.
            files.Add(new ArchiveEntry(source, source.Position, length, fname, date));
        }
    }

    public struct ArchiveEntry
    {
        public Stream Stream;
        public long Offset;
        public int Length;
        public string Filename;
        public DateTime Date;

        public ArchiveEntry(Stream stream, long offset, int length, string fname)
        {
            Stream = stream;
            Offset = offset;
            Length = length;
            Filename = fname;
            Date = DateTime.MinValue;
        }

        public ArchiveEntry(Stream stream, long offset, int length, string fname, DateTime date)
        {
            Stream = stream;
            Offset = offset;
            Length = length;
            Filename = fname;
            Date = date;
        }
    }

    public class ArchiveWriterSettings
    {
        // Global
        public int BlockSize = 16;

        // AFS
        public int AFSVersion = 2;

        // GVM/PVM/SVM
        public bool PVMFilename = true;
        public bool PVMFormats = true;
        public bool PVMDimensions = true;

        // SNT
        public int SNTType = 0; // 0 = PS2, 1 = PSP
    }
}