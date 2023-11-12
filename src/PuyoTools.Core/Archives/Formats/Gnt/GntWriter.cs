using PuyoTools.Core.Archives;
using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gnt
{
    public class GntWriter : ArchiveWriter<ArchiveWriterEntry>
    {
        public GntWriter(Stream destination) : base(destination)
        {
        }

        public override void Write(Stream destination)
        {
            // The start of the archive
            long streamStart = destination.Position;

            using BinaryWriter writer = new(destination, Encoding.UTF8, true);

            // Write the header.
            writer.Write(GntConstants.PrimaryMagicCode);

            writer.WriteInt32(24); // Unknown
            writer.WriteInt32BigEndian(1); // Unknown
            writer.WriteInt32BigEndian(32); // Unknown (offset of the NGTL chunk?)

            // Calculate the length of the NGTL chunk.
            int ngtlLength = _entries.Sum(x => MathHelper.RoundUp((int)x.Length, 8));

            writer.WriteInt32BigEndian(MathHelper.RoundUp(28 + (_entries.Count * 28), 8) + ngtlLength);
            writer.WriteInt32BigEndian(MathHelper.RoundUp(60 + (_entries.Count * 28), 8) + ngtlLength);
            writer.WriteInt32BigEndian(24 + (_entries.Count * 4));
            writer.WriteInt32BigEndian(1);

            // Write the NGTL chunk.
            writer.Write(GntConstants.SecondaryMagicCode);

            writer.WriteInt32(MathHelper.RoundUp(20 + (_entries.Count * 28), 8) + ngtlLength);
            writer.WriteInt32BigEndian(16);
            writer.WriteInt32BigEndian(0);
            writer.WriteInt32BigEndian(_entries.Count);
            writer.WriteInt32BigEndian(28);
            writer.WriteInt32BigEndian(28 + (_entries.Count * 20));

            for (int i = 0; i < _entries.Count; i++)
            {
                writer.WriteInt32BigEndian(0);
                writer.WriteInt32BigEndian(0);
                writer.Write(new byte[] { 0, 1, 0, 1 });
                writer.WriteInt32BigEndian(i);
                writer.WriteInt32BigEndian(0);
            }

            int entryOffset = 60 + (_entries.Count * 28);
            foreach (ArchiveWriterEntry entry in _entries)
            {
                writer.WriteInt32BigEndian((int)entry.Length);
                writer.WriteInt32BigEndian(entryOffset - 32);

                entryOffset += MathHelper.RoundUp((int)entry.Length, 8);
            }

            // Write the entry data.
            foreach (ArchiveWriterEntry entry in _entries)
            {
                WriteEntry(destination, entry);
                writer.Align(8, streamStart);
            }

            writer.Align(8, streamStart);

            writer.Write(GntConstants.Nof0MagicCode);

            writer.WriteInt32(MathHelper.RoundUp(28 + (_entries.Count * 4), 8));
            writer.WriteInt32BigEndian(_entries.Count + 2);
            writer.WriteInt32BigEndian(0);
            writer.WriteInt32BigEndian(20);

            for (int i = 0; i < _entries.Count; i++)
            {
                writer.WriteInt32BigEndian(32 + (_entries.Count * 20) + (i * 8));
            }

            writer.WriteInt32BigEndian(24);

            writer.Align(16, streamStart);

            writer.Write(GntConstants.NendMagicCode);

            writer.Align(16, streamStart);

            /*// Magic code "NGIF"
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
            while ((destination.Position - streamStart) % 8 != 0)
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
            while ((destination.Position - streamStart) % 16 != 0)
                destination.WriteByte(0);

            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'E');
            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'D');

            while ((destination.Position - streamStart) % 16 != 0)
                destination.WriteByte(0);*/
        }

        protected override ArchiveWriterEntry CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            return new ArchiveWriterEntry(source, null, leaveOpen);
        }
    }
}
