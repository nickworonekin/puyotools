using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class OneUnleashedArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'o', (byte)'n', (byte)'e', (byte)'.' };

        public override LegacyArchiveReader Open(Stream source)
        {
            return new OneUnleashedArchiveReader(source);
        }

        public override LegacyArchiveWriter Create(Stream destination)
        {
            return new OneUnleashedArchiveWriter(destination);
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
                return source.Length - startPosition > 8
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode);
            }
        }
    }

    #region Archive Reader
    public class OneUnleashedArchiveReader : LegacyArchiveReader
    {
        public OneUnleashedArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 4;
            int numEntries = PTStream.ReadInt32(source);
            entries = new List<ArchiveEntry>(numEntries);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry filename, offset, and length
                string entryFilename = PTStream.ReadCString(source, 56);
                int entryOffset = PTStream.ReadInt32(source);
                int entryLength = PTStream.ReadInt32(source);

                // Add this entry to the collection
                entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, entryFilename));
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class OneUnleashedArchiveWriter : LegacyArchiveWriter
    {
        public OneUnleashedArchiveWriter(Stream dest) : base(dest)
        {
        }

        protected override void WriteFile()
        {
            destination.WriteByte((byte)'o');
            destination.WriteByte((byte)'n');
            destination.WriteByte((byte)'e');
            destination.WriteByte((byte)'.');
            PTStream.WriteInt32(destination, entries.Count);

            var offset = MathHelper.RoundUp(8 + (entries.Count * 64), 32);

            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                var entry = entries[i];
                var entryStream = entry.Open();

                PTStream.WriteCString(destination, entry.Name, 56);
                PTStream.WriteInt32(destination, offset);
                PTStream.WriteInt32(destination, (int)entryStream.Length);

                Debug.Assert(destination.Position <= offset, "Table overrun!");

                var currentPos = destination.Position;
                destination.Position = offset;
                entryStream.CopyTo(destination);
                destination.Position = currentPos;

                offset = MathHelper.RoundUp(offset + (int)entryStream.Length, 64);

                // Call the entry written event
                OnEntryWritten(new ArchiveEntryWrittenEventArgs(entries[i]));
            }

            // Pad the end of the stream if necessary
            destination.Seek(0, SeekOrigin.End);
            while (destination.Position < offset)
                destination.WriteByte(0);
        }
    }
    #endregion
}