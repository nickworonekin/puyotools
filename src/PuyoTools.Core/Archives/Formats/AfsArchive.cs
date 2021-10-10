using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class AfsArchive : ArchiveBase
    {
        private static readonly byte[] magicCode = { (byte)'A', (byte)'F', (byte)'S', 0 };

        public override ArchiveReader Open(Stream source)
        {
            return new AfsArchiveReader(source);
        }

        public override ArchiveWriter Create(Stream destination)
        {
            return new AfsArchiveWriter(destination);
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
    public class AfsArchiveReader : ArchiveReader
    {
        public AfsArchiveReader(Stream source) : base(source)
        {
            // Get the number of entries in the archive
            source.Position += 4;
            int numEntries = PTStream.ReadInt32(source);
            entries = new List<ArchiveEntry>(numEntries);

            // Get the offset of the metadata
            source.Position += (numEntries * 8);
            int metadataOffset = PTStream.ReadInt32(source);

            // If the offset isn't stored there, then it is stored right before the offset of the first entry
            if (metadataOffset == 0)
            {
                source.Position = startOffset + 8;
                source.Position = PTStream.ReadInt32(source) - 8;
                metadataOffset = PTStream.ReadInt32(source);
            }

            // Read in all the entries
            for (int i = 0; i < numEntries; i++)
            {
                // Read in the entry offset and length
                source.Position = startOffset + 8 + (i * 8);
                int entryOffset = PTStream.ReadInt32(source);
                int entryLength = PTStream.ReadInt32(source);

                // Read in the entry file name
                source.Position = metadataOffset + (i * 48);
                string entryFname = PTStream.ReadCString(source, 32);

                // Add this entry to the collection
                entries.Add(new ArchiveEntry(this, startOffset + entryOffset, entryLength, entryFname));
            }

            // Set the position of the stream to the end of the file
            source.Seek(0, SeekOrigin.End);
        }
    }
    #endregion

    #region Archive Writer
    public class AfsArchiveWriter : ArchiveWriter
    {
        #region Settings
        /// <summary>
        /// The block size for this archive. The default value is 2048.
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

        /// <summary>
        /// The version of this archive to use. Use Version 1 for Dreamcast games and Version 2 for PS2, GCN, Xbox, and beyond.
        /// The default value is WriterSettings.AfsVersion.Version1.
        /// </summary>
        public AfsVersion Version
        {
            get { return version; }
            set
            {
                if (value != AfsVersion.Version1 && value != AfsVersion.Version2)
                {
                    throw new ArgumentOutOfRangeException("Version");
                }

                version = value;
            }
        }
        private AfsVersion version;

        public enum AfsVersion
        {
            Version1, // Dreamcast
            Version2, // Post Dreamcast (PS2, GC, Xbox and after)
        }

        /// <summary>
        /// Sets if each file should include a timestamp. The default value is true.
        /// </summary>
        public bool HasTimestamps { get; set; }
        #endregion

        public AfsArchiveWriter(Stream destination) : base(destination)
        {
            // Set default settings
            blockSize = 2048;
            version = AfsVersion.Version1;
            HasTimestamps = true;
        }

        protected override void WriteFile()
        {
            // The start of the archive
            long offset = destination.Position;

            // Magic code "AFS\0"
            destination.WriteByte((byte)'A');
            destination.WriteByte((byte)'F');
            destination.WriteByte((byte)'S');
            destination.WriteByte(0);

            // Number of entries in the archive
            PTStream.WriteInt32(destination, entries.Count);

            // Write out the header for the archive
            int entryOffset = PTMethods.RoundUp(12 + (entries.Count * 8), blockSize);
            int firstEntryOffset = entryOffset;

            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteInt32(destination, entryOffset);
                PTStream.WriteInt32(destination, entries[i].Length);

                entryOffset += PTMethods.RoundUp(entries[i].Length, blockSize);
            }

            // If this is AFS v1, then the metadata offset is stored at 8 bytes before
            // the first entry offset.
            if (version == AfsVersion.Version1)
            {
                destination.Position = offset + firstEntryOffset - 8;
            }

            // Write out the metadata offset and length
            PTStream.WriteInt32(destination, entryOffset);
            PTStream.WriteInt32(destination, entries.Count * 48);

            destination.Position = offset + firstEntryOffset;

            // Write out the file data for each entry
            for (int i = 0; i < entries.Count; i++)
            {
                // Call the entry writing event
                OnEntryWriting(new ArchiveEntryWritingEventArgs(entries[i]));

                PTStream.CopyToPadded(entries[i].Open(), destination, blockSize, 0);

                // Call the entry written event
                OnEntryWritten(new ArchiveEntryWrittenEventArgs(entries[i]));
            }

            // Write out the footer for the archive
            for (int i = 0; i < entries.Count; i++)
            {
                PTStream.WriteCString(destination, entries[i].Name, 32);

                // File timestamp
                if (HasTimestamps && File.Exists(entries[i].Path))
                {
                    // File exists, let's read in the file timestamp
                    FileInfo fileInfo = new FileInfo(entries[i].Path);
                    var lastWriteTime = fileInfo.LastWriteTime;

                    PTStream.WriteInt16(destination, (short)lastWriteTime.Year);
                    PTStream.WriteInt16(destination, (short)lastWriteTime.Month);
                    PTStream.WriteInt16(destination, (short)lastWriteTime.Day);
                    PTStream.WriteInt16(destination, (short)lastWriteTime.Hour);
                    PTStream.WriteInt16(destination, (short)lastWriteTime.Minute);
                    PTStream.WriteInt16(destination, (short)lastWriteTime.Second);
                }
                else
                {
                    // File does not exist, just store all 0s
                    PTStream.WriteInt16(destination, 0);
                    PTStream.WriteInt16(destination, 0);
                    PTStream.WriteInt16(destination, 0);
                    PTStream.WriteInt16(destination, 0);
                    PTStream.WriteInt16(destination, 0);
                    PTStream.WriteInt16(destination, 0);
                }

                // Write out this data that I have no idea what its purpose is
                long oldPosition = destination.Position;
                byte[] buffer = new byte[4];

                if (version == AfsVersion.Version1)
                    destination.Position = offset + 8 + (i * 8);
                else
                    destination.Position = offset + 4 + (i * 4);

                destination.Read(buffer, 0, 4);
                destination.Position = oldPosition;
                destination.Write(buffer, 0, 4);
            }

            // Finish padding out the archive
            while ((destination.Position - offset) % blockSize != 0)
                destination.WriteByte(0);
        }
    }
    #endregion
}