using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class AcxArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { 0, 0, 0, 0 };

        public override LegacyArchiveReader Open(Stream source)
        {
            return new AcxArchiveReader(source);
        }

        public override LegacyArchiveWriter Create(Stream destination)
        {
            return new AcxArchiveWriter(destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;
            var remainingLength = source.Length - startPosition;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                // Verify the magic code
                if (!(remainingLength > 12
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode)))
                {
                    return false;
                }

                // Since ACX archives don't have an easy way to identify them outside of their file extension,
                // we'll check to see if the offset of the first file is where it's expected to be.
                var numEntries = reader.At(startPosition + 4, x => x.ReadInt32BigEndian());
                var actualOffset = reader.At(startPosition + 8, x => x.ReadInt32BigEndian());
                var expectedOffset = 8 + (numEntries * 8);

                // Verify the offset of the first file
                if (remainingLength > expectedOffset
                    && (actualOffset == expectedOffset
                        || actualOffset % 2048 == 0))
                {
                    return true;
                }

                return false;
            }
        }
    }

    #region Archive Reader
    public class AcxArchiveReader : LegacyArchiveReader
    {
        public AcxArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 4;
            int numEntries = PTStream.ReadInt32BE(source);
            entries = new List<ArchiveEntry>(numEntries);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry offset and length
                int entryOffset = PTStream.ReadInt32BE(source);
                int entryLength = PTStream.ReadInt32BE(source);

                // Add this entry to the collection
                entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, string.Empty));
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class AcxArchiveWriter : LegacyArchiveWriter
    {
        #region Settings
        /// <summary>
        /// The block size for this archive. The default value is 4.
        /// </summary>
        public int BlockSize
        {
            get { return blockSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("BlockSize");
                }

                blockSize = value;
            }
        }
        private int blockSize;
        #endregion

        public AcxArchiveWriter(Stream destination) : base(destination)
        {
            // Set default settings
            blockSize = 4;
        }

        protected override void WriteFile()
        {
            // The start of the archive
            long offset = destination.Position;

            // Magic code "\0\0\0\0"
            destination.WriteByte(0);
            destination.WriteByte(0);
            destination.WriteByte(0);
            destination.WriteByte(0);

            // Number of entries in the archive
            PTStream.WriteInt32BE(destination, entries.Count);

            // Write out the header for the archive
            int entryOffset = PTMethods.RoundUp(8 + (entries.Count * 8), blockSize);
            int firstEntryOffset = entryOffset;
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32BE(destination, entryOffset);
                PTStream.WriteInt32BE(destination, entries[i].Length);

                entryOffset += PTMethods.RoundUp(entries[i].Length, blockSize);
            }

            // Pad before writing out the file data
            while ((destination.Position - offset) % blockSize != 0)
                destination.WriteByte(0);

            // Write out the file data for each entry
            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                PTStream.CopyToPadded(entries[i].Open(), destination, blockSize, 0);

                // Call the entry written event
                OnEntryWritten(new ArchiveEntryWrittenEventArgs(entries[i]));
            }
        }
    }
    #endregion
}