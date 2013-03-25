using System;
using System.IO;
using System.Collections.Generic;

namespace PuyoTools.Archive
{
    public abstract class ArchiveBase
    {
        public abstract ArchiveReader Open(Stream source, int length);
        public abstract ArchiveWriter Create(Stream destination, ArchiveWriterSettings settings);
        public abstract bool Is(Stream source, int length, string fname);

        // It's assumed that this format can be read (in this case, opened).
        // So, we're only going to bother seeing if this format can be written to (created).
        public abstract bool CanCreate();

        public ArchiveWriter Create(Stream destination)
        {
            return Create(destination, null);
        }
    }

    public abstract class ArchiveReader
    {
        public ArchiveEntry[] Files;
        protected long archiveOffset;

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

        public abstract void Flush();

        protected void Initalize(Stream destination)
        {
            files = new List<ArchiveEntry>();
            this.destination = destination;
        }

        public void AddFile(Stream source, string fname)
        {
            AddFile(source, (int)(source.Length - source.Position), fname);
        }

        public void AddFile(Stream source, string fname, string sourceFile)
        {
            AddFile(source, (int)(source.Length - source.Position), fname, sourceFile);
        }

        public virtual void AddFile(Stream source, int length, string fname)
        {
            // All we're going to do is add the file to the entry list
            // The magic happens once Flush is called.
            files.Add(new ArchiveEntry(source, source.Position, length, fname));
        }

        public void AddFile(Stream source, int length, string fname, string sourceFile)
        {
            // All we're going to do is add the file to the entry list
            // The magic happens once Flush is called.
            files.Add(new ArchiveEntry(source, source.Position, length, fname, sourceFile));
        }
    }

    public struct ArchiveEntry
    {
        public Stream Stream;
        public long Offset;
        public int Length;
        public string Filename;
        public string SourceFile;

        public ArchiveEntry(Stream stream, long offset, int length, string fname)
        {
            Stream = stream;
            Offset = offset;
            Length = length;
            Filename = fname;
            SourceFile = String.Empty;
        }

        public ArchiveEntry(Stream stream, long offset, int length, string fname, string sourceFile)
        {
            Stream = stream;
            Offset = offset;
            Length = length;
            Filename = fname;
            SourceFile = sourceFile;
        }
    }

    // We're going to declare an empty ArchiveWriterSettings here. Why is it empty?
    // That way we can split up the settings and the panel content for the archive
    // creator. If someone is using the archive code for their projects, they don't
    // need the things for the panel content.
    public abstract partial class ArchiveWriterSettings { }

    public class CannotAddFileToArchiveException : Exception { }
}