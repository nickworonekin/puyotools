using System;
using System.IO;

namespace PuyoTools.Modules.Archive
{
    public class GntArchive : ArchiveBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "GNT"; }
        }

        /// <summary>
        /// The primary file extension for this archive format.
        /// </summary>
        public override string FileExtension
        {
            get { return ".gnt"; }
        }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        public override ArchiveReader Open(Stream source)
        {
            return new GntArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new GntArchiveWriter(destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 36
                && PTStream.Contains(source, 0, new byte[] { (byte)'N', (byte)'G', (byte)'I', (byte)'F' })
                && PTStream.Contains(source, 32, new byte[] { (byte)'N', (byte)'G', (byte)'T', (byte)'L' }));
        }
    }

    #region Archive Reader
    public class GntArchiveReader : ArchiveReader
    {
        public GntArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 48;
            int numEntries = PTStream.ReadInt32BE(source);
            entries = new ArchiveEntryCollection(this, numEntries);

            source.Position += 8 + (numEntries * 20);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry offset and length
                int entryLength = PTStream.ReadInt32BE(source);
                int entryOffset = PTStream.ReadInt32BE(source) + 32;

                // Add this entry to the collection
                entries.Add(startOffset + entryOffset, entryLength, String.Empty);
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class GntArchiveWriter : ArchiveWriter
    {
        public GntArchiveWriter(Stream destination) : base(destination) { }

        public override void Flush()
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
                PTStream.CopyToPadded(entries[i].Open(), destination, 8, 0);

                // Call the file added event
                OnFileAdded(EventArgs.Empty);
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