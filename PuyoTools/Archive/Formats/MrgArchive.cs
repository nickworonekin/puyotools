using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Archive
{
    public class MrgArchive : ArchiveBase
    {
        public override ArchiveReader Open(Stream source, int length)
        {
            return new Read(source, length);
        }

        public override ArchiveWriter Create(Stream destination, ArchiveWriterSettings settings)
        {
            return new Write(destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 16 && PTStream.Contains(source, 0, new byte[] { (byte)'M', (byte)'R', (byte)'G', (byte)'0' }));
        }

        public override bool CanCreate()
        {
            return true;
        }

        public class Read : ArchiveReader
        {
            public Read(Stream source, int length)
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

                    source.Position += 4;

                    string entryFilename = PTStream.ReadCString(source, 32, Encoding.GetEncoding("Shift_JIS"));

                    if (entryFileExtension != String.Empty)
                        entryFileExtension = "." + entryFileExtension;

                    // Add this entry to the file list
                    Files[i] = new ArchiveEntry(source, archiveOffset + entryOffset, entryLength, entryFilename + entryFileExtension);
                }

                // Set the position of the stream to the end of the file
                source.Position = archiveOffset + length;
            }
        }

        public class Write : ArchiveWriter
        {
            public Write(Stream destination)
            {
                Initalize(destination);
            }

            public Write(Stream destination, ArchiveWriterSettings settings) : this(destination) { }

            public override void Flush()
            {
                // The start of the archive
                long offset = destination.Position;

                // Magic code "MRG0"
                destination.WriteByte((byte)'M');
                destination.WriteByte((byte)'R');
                destination.WriteByte((byte)'G');
                destination.WriteByte((byte)'0');

                // Number of files in the archive
                PTStream.WriteInt32(destination, files.Count);

                destination.Position += 8;

                // Write out the header for the archive
                int entryOffset = 16 + (files.Count * 48);
                
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

                    destination.Position += 4;

                    PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(files[i].Filename), 32, Encoding.GetEncoding("Shift_JIS"));

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