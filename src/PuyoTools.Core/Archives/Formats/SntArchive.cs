using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PuyoTools.Core.Textures;

namespace PuyoTools.Core.Archives
{
    public class SntArchive : ArchiveBase
    {
        private static readonly byte[] ps2PrimaryMagicCode = { (byte)'N', (byte)'S', (byte)'I', (byte)'F' };
        private static readonly byte[] ps2SecondaryMagicCode = { (byte)'N', (byte)'S', (byte)'T', (byte)'L' };
        private static readonly byte[] pspPrimaryMagicCode = { (byte)'N', (byte)'U', (byte)'I', (byte)'F' };
        private static readonly byte[] pspSecondaryMagicCode = { (byte)'N', (byte)'U', (byte)'T', (byte)'L' };

        public override ArchiveReader Open(Stream source)
        {
            return new SntArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new SntArchiveWriter(destination);
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
                if (!(source.Length - startPosition > 36
                    && reader.At(startPosition + 8, x => x.ReadInt32()) == 1))
                {
                    return false;
                }

                var primaryMagicCode = reader.At(startPosition, x => x.ReadBytes(4));
                var secondaryMagicCode = reader.At(startPosition + 32, x => x.ReadBytes(4));

                // Verify PS2 magic code
                if (primaryMagicCode.SequenceEqual(ps2PrimaryMagicCode)
                    && secondaryMagicCode.SequenceEqual(ps2SecondaryMagicCode))
                {
                    return true;
                }

                // Verify PSP magic code
                if (primaryMagicCode.SequenceEqual(pspPrimaryMagicCode)
                    && secondaryMagicCode.SequenceEqual(pspSecondaryMagicCode))
                {
                    return true;
                }

                return false;
            }
        }
    }

    #region Archive Reader
    public class SntArchiveReader : ArchiveReader
    {
        public SntArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 48;
            int numEntries = PTStream.ReadInt32(source);
            entries = new List<ArchiveEntry>(numEntries);

            source.Position += 8 + (numEntries * 20);

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Readin the entry offset and length
                int entryLength = PTStream.ReadInt32(source);
                int entryOffset = PTStream.ReadInt32(source) + 32;

                // If this archive contains GIM textures, then it's possible that they may contain filenames.
                // Let's check and see
                string entryFname = String.Empty;
                if (entryLength > 40)
                {
                    long oldPosition = source.Position;
                    source.Position = startOffset + entryOffset;

                    if (GimTexture.Identify(new StreamView(source, entryLength)))
                    {
                        // It's a GIM texture. Let's try to find a filename
                        source.Position += 36;
                        long fnameOffset = PTStream.ReadInt32(source) + 48;

                        if (fnameOffset < source.Length)
                        {
                            source.Position = startOffset + entryOffset + fnameOffset;
                            entryFname = Path.GetFileNameWithoutExtension(PTStream.ReadCString(source, (int)(source.Length - fnameOffset)));

                            if (entryFname != String.Empty)
                            {
                                entryFname += ".gim";
                            }
                        }
                    }

                    source.Position = oldPosition;
                }

                // Add this entry to the collection
                entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, entryFname));
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class SntArchiveWriter : ArchiveWriter
    {
        #region Settings
        /// <summary>
        /// The platform this archive is to be used on. The default value is WriterSettings.SntType.Ps2
        /// </summary>
        public SntPlatform Platform
        {
            get { return platform; }
            set
            {
                if (value != SntPlatform.Ps2 && value != SntPlatform.Psp)
                {
                    throw new ArgumentOutOfRangeException("Platform");
                }

                platform = value;
            }
        }
        private SntPlatform platform;

        public enum SntPlatform
        {
            Ps2, // PlayStation 2
            Psp, // PlayStation Portable
        }
        #endregion

        public SntArchiveWriter(Stream destination) : base(destination)
        {
            // Set default settings
            platform = SntPlatform.Ps2;
        }

        protected override void WriteFile()
        {
            // The start of the archive
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
                destination.WriteByte(0);
        }
    }
    #endregion
}