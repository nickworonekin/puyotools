using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PuyoTools.Core;

namespace PuyoTools.Archives.Formats.Vdd
{
    public class VddReader : ArchiveReader<ArchiveReaderEntry>
    {
        public VddReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a VDD archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            int numEntries = reader.ReadInt32();
            _entries = new List<ArchiveReaderEntry>(numEntries);

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                string name = reader.ReadString(16);
                long offset = reader.ReadInt32() * 2048;
                int length = reader.ReadInt32();

                // Add this entry to the entry collection.
                _entries.Add(new ArchiveReaderEntry(_stream, _streamStart + offset, length, name));
            }

            // We're done reading this archive. Seek to the end of the stream.
            _stream.Seek(0, SeekOrigin.End);
        }

        /// <summary>
        /// Returns if the data in <paramref name="source"/> resembles an archive of this format.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns><see langword="true"/> if the data resembles an archive of this format, <see langword="false"/> otherwise.</returns>
        public static bool IsFormat(Stream source)
        {
            // VDD archives don't have an easy way to identify them outside of their file extension.
            // To best identify them, we will:
            // - Treat archives with no entries as invalid.
            // - Verify the first entry is at the expected offset.
            // - Verify the last entry is at the end of the archive.

            long startPosition = source.Position;
            long remainingLength = source.Length - startPosition;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            if (remainingLength < 4)
            {
                return false;
            }

            // Get the number of entries. If it's 0 (or negative), treat it as invalid.
            int numEntries = reader.At(startPosition, x => x.ReadInt32());

            if (numEntries < 1 || remainingLength < 4 + (numEntries * 24))
            {
                return false;
            }

            // Get the offset of the first entry, and verify it matches the expected offset.
            long firstEntryOffset = reader.At(startPosition + 20, x => x.ReadInt32()) * 2048;
            long expectedFirstEntryOffset = long.RoundUp(4 + (numEntries * 24), 2048);

            if (firstEntryOffset != expectedFirstEntryOffset)
            {
                return false;
            }

            // Get the offset & length of the last entry, and verify it's at the end of the archive.
            long lastEntryOffset = reader.At(startPosition + 4 + ((numEntries - 1) * 24) + 16, x => x.ReadInt32()) * 2048;
            int lastEntryLength = reader.At(startPosition + 4 + ((numEntries - 1) * 24) + 20, x => x.ReadInt32());

            if (lastEntryOffset + int.RoundUp(lastEntryLength, 2048) != remainingLength)
            {
                return false;
            }

            return true;
        }
    }
}
