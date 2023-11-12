using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class GntArchive : ArchiveBase
    {
        private static readonly byte[] primaryMagicCode = { (byte)'N', (byte)'G', (byte)'I', (byte)'F' };
        private static readonly byte[] secondaryMagicCode = { (byte)'N', (byte)'G', (byte)'T', (byte)'L' };

        public override LegacyArchiveReader Open(Stream source)
        {
            return new GntArchiveReader(source);
        }

        public override LegacyArchiveWriter Create(Stream destination)
        {
            return new GntArchiveWriter(destination);
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
                return source.Length - startPosition > 36
                    && reader.At(startPosition, x => x.ReadBytes(primaryMagicCode.Length)).SequenceEqual(primaryMagicCode)
                    && reader.At(startPosition + 8, x => x.ReadInt32BigEndian()) == 1 // Check so we don't mis-identify GNO archives from the Sonic Storybook games as GNT archives
                    && reader.At(startPosition + 32, x => x.ReadBytes(secondaryMagicCode.Length)).SequenceEqual(secondaryMagicCode);
            }
        }
    }

    #region Archive Reader
    public class GntArchiveReader : LegacyArchiveReader
    {
        public GntArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 48;
            int numEntries = PTStream.ReadInt32BE(source);
            entries = new List<ArchiveEntry>(numEntries);

            source.Position += 8 + (numEntries * 20);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry offset and length
                int entryLength = PTStream.ReadInt32BE(source);
                int entryOffset = PTStream.ReadInt32BE(source) + 32;

                // Add this entry to the collection
                entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, string.Empty));
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class GntArchiveWriter : LegacyArchiveWriter
    {
        public GntArchiveWriter(Stream destination) : base(destination) { }

        protected override void WriteFile()
        {
            // The start of the archive
            long offset = destination.Position;

            // Magic code "NGIF"
            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'G');
            destination.WriteByte((byte)'I');
            destination.WriteByte((byte)'F');

            PTStream.WriteInt32(destination, 24); // Unknown
            PTStream.WriteInt32BE(destination, 1); // Unknown
            PTStream.WriteInt32BE(destination, 32); // Offset of the NGTL chunk?

            // Calculate the size of the NGTL chunk
            int NGTLLength = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                NGTLLength += PTMethods.RoundUp(entries[i].Length, 8);
            }

            PTStream.WriteInt32BE(destination, PTMethods.RoundUp(28 + (entries.Count * 28), 8) + NGTLLength);
            PTStream.WriteInt32BE(destination, PTMethods.RoundUp(60 + (entries.Count * 28), 8) + NGTLLength);
            PTStream.WriteInt32BE(destination, 24 + (entries.Count * 4));
            PTStream.WriteInt32BE(destination, 1);

            // NGTL chunk
            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'G');
            destination.WriteByte((byte)'T');
            destination.WriteByte((byte)'L');

            PTStream.WriteInt32(destination, PTMethods.RoundUp(20 + (entries.Count * 28), 8) + NGTLLength);
            PTStream.WriteInt32BE(destination, 16);
            PTStream.WriteInt32BE(destination, 0);
            PTStream.WriteInt32BE(destination, entries.Count);
            PTStream.WriteInt32BE(destination, 28);
            PTStream.WriteInt32BE(destination, 28 + (entries.Count * 20));

            // Write out crap bytes
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32BE(destination, 0);
                PTStream.WriteInt32BE(destination, 0);
                destination.Write(new byte[] { 0, 1, 0, 1 }, 0, 4);
                PTStream.WriteInt32BE(destination, i);
                PTStream.WriteInt32BE(destination, 0);
            }

            // Write out the header for the archive
            int entryOffset = 60 + (entries.Count * 28);
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32BE(destination, entries[i].Length);
                PTStream.WriteInt32BE(destination, entryOffset - 32);

                entryOffset += PTMethods.RoundUp(entries[i].Length, 8);
            }

            // Write out the file data for each entry
            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                PTStream.CopyToPadded(entries[i].Open(), destination, 8, 0);

                // Call the entry written event
                OnEntryWritten(new ArchiveEntryWrittenEventArgs(entries[i]));
            }

            // Pad before writing out the NOF0 chunk
            while ((destination.Position - offset) % 8 != 0)
                destination.WriteByte(0);

            // NOF0 chunk
            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'O');
            destination.WriteByte((byte)'F');
            destination.WriteByte((byte)'0');

            // Write out crap bytes
            PTStream.WriteInt32(destination, PTMethods.RoundUp(28 + (entries.Count * 4), 8));
            PTStream.WriteInt32BE(destination, entries.Count + 2);
            PTStream.WriteInt32BE(destination, 0);
            PTStream.WriteInt32BE(destination, 20);

            // Write out more unknown stuff
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32BE(destination, 32 + (entries.Count * 20) + (i * 8));
            }

            PTStream.WriteInt32BE(destination, 24);

            // Pad before we write NEND
            // Finish padding out the archive
            while ((destination.Position - offset) % 16 != 0)
                destination.WriteByte(0);

            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'E');
            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'D');

            while ((destination.Position - offset) % 16 != 0)
                destination.WriteByte(0);
        }
    }
    #endregion
}