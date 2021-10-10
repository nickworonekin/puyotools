using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class MrgArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'M', (byte)'R', (byte)'G', (byte)'0' };

        public override ArchiveReader Open(Stream source)
        {
            return new MrgArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new MrgArchiveWriter(destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return source.Length - startPosition > 16
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode);
            }
        }
    }

    #region Archive Reader
    public class MrgArchiveReader : ArchiveReader
    {
        public MrgArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 4;
            int numEntries = PTStream.ReadInt32(source);
            entries = new List<ArchiveEntry>(numEntries);

            source.Position += 8;

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry filename extension, offset, length, and filename without the extension
                string entryFileExtension = PTStream.ReadCString(source, 4, EncodingExtensions.ShiftJIS);
                int entryOffset = PTStream.ReadInt32(source);
                int entryLength = PTStream.ReadInt32(source);

                source.Position += 4;

                string entryFilename = PTStream.ReadCString(source, 32, EncodingExtensions.ShiftJIS);

                if (entryFileExtension != String.Empty)
                    entryFileExtension = "." + entryFileExtension;

                // Add this entry to the collection
                entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, entryFilename + entryFileExtension));
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class MrgArchiveWriter : ArchiveWriter
    {
        public MrgArchiveWriter(Stream destination) : base(destination) { }

        protected override void WriteFile()
        {
            // The start of the archive
            long offset = destination.Position;

            // Magic code "MRG0"
            destination.WriteByte((byte)'M');
            destination.WriteByte((byte)'R');
            destination.WriteByte((byte)'G');
            destination.WriteByte((byte)'0');

            // Number of entries in the archive
            PTStream.WriteInt32(destination, entries.Count);

            destination.Position += 8;

            // Write out the header for the archive
            int entryOffset = 16 + (entries.Count * 48);

            for (int i = 0; i < entries.Count; i++)
            {
                // Write out the file extension
                string fileExtension = Path.GetExtension(entries[i].Name);
                if (fileExtension != String.Empty)
                    fileExtension = fileExtension.Substring(1);

                PTStream.WriteCString(destination, fileExtension, 4, EncodingExtensions.ShiftJIS);

                // Write out the offset, length, and filename (without the extension)
                PTStream.WriteInt32(destination, entryOffset);
                PTStream.WriteInt32(destination, entries[i].Length);

                destination.Position += 4;

                PTStream.WriteCString(destination, Path.GetFileNameWithoutExtension(entries[i].Name), 32, EncodingExtensions.ShiftJIS);

                entryOffset += PTMethods.RoundUp(entries[i].Length, 16);
            }

            // Write out the file data for each entry
            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                PTStream.CopyToPadded(entries[i].Open(), destination, 16, 0);

                // Call the entry written event
                OnEntryWritten(new ArchiveEntryWrittenEventArgs(entries[i]));
            }
        }
    }
    #endregion
}