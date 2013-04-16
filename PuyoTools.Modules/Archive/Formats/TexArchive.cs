using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Modules.Archive
{
    public class TexArchive : ArchiveBase
    {
        public override string Name
        {
            get { return "TEX"; }
        }

        public override string FileExtension
        {
            get { return ".tex"; }
        }

        public override bool CanCreate
        {
            get { return true; }
        }

        public override ArchiveReader Open(Stream source, int length)
        {
            return new Reader(source, length);
        }

        public override ArchiveWriter Create(Stream destination, ArchiveWriterSettings settings)
        {
            return new Writer(destination);
        }

        public override ArchiveWriterSettings GetWriterSettings()
        {
            return null;
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && PTStream.Contains(source, 0, new byte[] { (byte)'T', (byte)'E', (byte)'X', (byte)'0' }));
        }

        public class Reader : ArchiveReader
        {
            public Reader(Stream source, int length)
            {
                // The start of the archive
                archiveOffset = source.Position;

                // Get the number of files in the archive
                source.Position += 4;
                int numFiles = PTStream.ReadInt32(source);
                Files = new ArchiveEntry[numFiles];

                source.Position += 8;

                // Read in all the file entries
                for (int i = 0; i < numFiles; i++)
                {
                    // Read in the entry filename extension, offset, length, and filename without the extension
                    string entryFileExtension = PTStream.ReadCString(source, 4, Encoding.GetEncoding("Shift_JIS"));
                    int entryOffset = PTStream.ReadInt32(source);
                    int entryLength = PTStream.ReadInt32(source);
                    string entryFilename = PTStream.ReadCString(source, 20, Encoding.GetEncoding("Shift_JIS"));

                    if (entryFileExtension != String.Empty)
                        entryFileExtension = "." + entryFileExtension;

                    // Add this entry to the file list
                    Files[i] = new ArchiveEntry(source, archiveOffset + entryOffset, entryLength, entryFilename + entryFileExtension);
                }

                // Set the position of the stream to the end of the file
                source.Position = archiveOffset + length;
            }
        }

        public class Writer : ArchiveWriter
        {
            public Writer(Stream destination)
            {
                Initalize(destination);
            }

            public Writer(Stream destination, ArchiveWriterSettings settings) : this(destination) { }

            public override void Flush()
            {
                // The start of the archive
                long offset = destination.Position;

                // Magic code "TEX0"
                destination.WriteByte((byte)'T');
                destination.WriteByte((byte)'E');
                destination.WriteByte((byte)'X');
                destination.WriteByte((byte)'0');

                // Number of files in the archive
                PTStream.WriteInt32(destination, files.Count);

                destination.Position += 8;

                // Write out the header for the archive
                int entryOffset = 16 + (files.Count * 32);

                for (int i = 0; i < files.Count; i++)
                {
                    // Write out the file extension
                    string fileExtension = Path.GetExtension(files[i].Filename);
                    if (fileExtension != String.Empty)
                        fileExtension = fileExtension.Substring(1);

                    PTStream.WriteCString(destination, fileExtension, 4, Encoding.GetEncoding("Shift_JIS"));

                    // Write out the offset, length, and filename (without the extension)
                    PTStream.WriteInt32(destination, entryOffset);
                    PTStream.WriteInt32(destination, files[i].Length);
                    PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(files[i].Filename), 20, Encoding.GetEncoding("Shift_JIS"));

                    entryOffset += PTMethods.RoundUp(files[i].Length, 16);
                }

                // Write out the file data for each file
                for (int i = 0; i < files.Count; i++)
                {
                    PTStream.CopyPartToPadded(files[i].Stream, destination, files[i].Length, 16, 0);
                }
            }
        }
    }
}