using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using PuyoTools.Core;

namespace PuyoTools.Archives.Formats.Snt
{
    public class SntWriter : ArchiveWriter<ArchiveWriterEntry>
    {
        private SntPlatform _platform = SntPlatform.PlayStation2;

        public SntWriter(Stream destination) : base(destination)
        {
        }

        public SntPlatform Platform
        {
            get => _platform;
            set
            {
                InvalidEnumArgumentException.ThrowIfNotDefined(value);

                _platform = value;
            }
        }

        public override void Write(Stream destination)
        {
            using BinaryWriter writer = new(new OffsetStream(destination));

            // Write the header.
            if (_platform == SntPlatform.PlayStation2)
            {
                writer.Write(SntConstants.Ps2PrimaryMagicCode);
            }
            else
            {
                writer.Write(SntConstants.PspPrimaryMagicCode);
            }

            writer.WriteInt32(24); // Unknown
            writer.WriteInt32(1); // Unknown
            writer.WriteInt32(32); // Unknown (offset of the NSTL/NUTL chunk?)

            // Calculate the length of the NSTL/NUTL chunk.
            int nstlLength = _entries.Sum(x => int.RoundUp((int)x.Length, 8));

            writer.WriteInt32(int.RoundUp(28 + (_entries.Count * 28), 8) + nstlLength);
            writer.WriteInt32(int.RoundUp(60 + (_entries.Count * 28), 8) + nstlLength);
            writer.WriteInt32(24 + (_entries.Count * 4));
            writer.WriteInt32(1);

            // Write the NSTL/NUTL chunk.
            if (_platform == SntPlatform.PlayStation2)
            {
                writer.Write(SntConstants.Ps2SecondaryMagicCode);
            }
            else
            {
                writer.Write(SntConstants.PspSecondaryMagicCode);
            }

            writer.WriteInt32(int.RoundUp(20 + (_entries.Count * 28), 8) + nstlLength);
            writer.WriteInt32(16);
            writer.WriteInt32(0);
            writer.WriteInt32(_entries.Count);
            writer.WriteInt32(28);
            writer.WriteInt32(28 + (_entries.Count * 20));

            for (int i = 0; i < _entries.Count; i++)
            {
                writer.WriteInt32(1);
                writer.WriteInt32(0);
                writer.Write([1, 0, 1, 0]);
                writer.WriteInt32(i);
                writer.WriteInt32(0);
            }

            int entryOffset = 60 + (_entries.Count * 28);
            foreach (ArchiveWriterEntry entry in _entries)
            {
                writer.WriteInt32((int)entry.Length);
                writer.WriteInt32(entryOffset - 32);

                entryOffset += int.RoundUp((int)entry.Length, 4);
            }

            // Write the entry data.
            foreach (ArchiveWriterEntry entry in _entries)
            {
                WriteEntry(writer.BaseStream, entry);
                writer.Align(4);
            }

            writer.Align(8);

            writer.Write(SntConstants.Nof0MagicCode);

            writer.WriteInt32(int.RoundUp(28 + (_entries.Count * 4), 8));
            writer.WriteInt32(_entries.Count + 2);
            writer.WriteInt32(0);
            writer.WriteInt32(20);

            for (int i = 0; i < _entries.Count; i++)
            {
                writer.WriteInt32(32 + (_entries.Count * 20) + (i * 8));
            }

            writer.WriteInt32(24);

            writer.Align(16);

            writer.Write(SntConstants.NendMagicCode);

            writer.Align(16);

            /*// The start of the archive
            long offset = destination.Position;

            // Magic code "NSIF/NUIF"
            if (platform == SntPlatform.Ps2)
            {
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'S');
                destination.WriteByte((byte)'I');
                destination.WriteByte((byte)'F');
            }
            else
            {
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'U');
                destination.WriteByte((byte)'I');
                destination.WriteByte((byte)'F');
            }

            PTStream.WriteInt32(destination, 24); // Unknown
            PTStream.WriteInt32(destination, 1); // Unknown
            PTStream.WriteInt32(destination, 32); // Offset of the NSTL/NUTL chunk?

            // Calculate the size of the NSTL chunk
            int NSTLLength = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                NSTLLength += PTMethods.RoundUp(entries[i].Length, 8);
            }

            PTStream.WriteInt32(destination, PTMethods.RoundUp(28 + (entries.Count * 28), 8) + NSTLLength);
            PTStream.WriteInt32(destination, PTMethods.RoundUp(60 + (entries.Count * 28), 8) + NSTLLength);
            PTStream.WriteInt32(destination, 24 + (entries.Count * 4));
            PTStream.WriteInt32(destination, 1);

            // NSTL/NUTL chunk
            if (platform == SntPlatform.Ps2)
            {
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'S');
                destination.WriteByte((byte)'T');
                destination.WriteByte((byte)'L');
            }
            else
            {
                destination.WriteByte((byte)'N');
                destination.WriteByte((byte)'U');
                destination.WriteByte((byte)'T');
                destination.WriteByte((byte)'L');
            }

            PTStream.WriteInt32(destination, PTMethods.RoundUp(20 + (entries.Count * 28), 8) + NSTLLength);
            PTStream.WriteInt32(destination, 16);
            PTStream.WriteInt32(destination, 0);
            PTStream.WriteInt32(destination, entries.Count);
            PTStream.WriteInt32(destination, 28);
            PTStream.WriteInt32(destination, 28 + (entries.Count * 20));

            // Write out crap bytes
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32(destination, 1);
                PTStream.WriteInt32(destination, 0);
                destination.Write(new byte[] { 1, 0, 1, 0 }, 0, 4);
                PTStream.WriteInt32(destination, i);
                PTStream.WriteInt32(destination, 0);
            }

            // Write out the header for the archive
            int entryOffset = 60 + (entries.Count * 28);
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32(destination, entries[i].Length);
                PTStream.WriteInt32(destination, entryOffset - 32);

                entryOffset += PTMethods.RoundUp(entries[i].Length, 4);
            }

            // Write out the file data for each entry
            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                PTStream.CopyToPadded(entries[i].Open(), destination, 4, 0);

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
            PTStream.WriteInt32(destination, entries.Count + 2);
            PTStream.WriteInt32(destination, 0);
            PTStream.WriteInt32(destination, 20);

            // Write out more unknown stuff
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32(destination, 32 + (entries.Count * 20) + (i * 8));
            }

            PTStream.WriteInt32(destination, 24);

            // Pad before we write NEND
            // Finish padding out the archive
            while ((destination.Position - offset) % 16 != 0)
                destination.WriteByte(0);

            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'E');
            destination.WriteByte((byte)'N');
            destination.WriteByte((byte)'D');

            while ((destination.Position - offset) % 16 != 0)
                destination.WriteByte(0);*/
        }

        protected override ArchiveWriterEntry CreateEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            return new ArchiveWriterEntry(source, null, leaveOpen);
        }
    }
}
