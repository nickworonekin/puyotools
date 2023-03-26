using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gvm
{
    /// <summary>
    /// Archive reader for GVM archives.
    /// </summary>
    public class GvmReader : ArchiveReader<GvmReaderEntry>
    {
        private bool _hasFilenames;
        private bool _hasFormats;
        private bool _hasDimensions;
        private bool _hasGlobalIndexes;
        private int _tableEntryLength;
        private int _globalIndexOffset;

        public GvmReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a GVM archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the offset of the first entry.
            _stream.Position += 4;
            int entryOffset = reader.ReadInt32() + 8;
            int headerOffset = 0xC;

            // Read the properties included in the archive.
            GvmFlags properties = (GvmFlags)reader.ReadUInt16BigEndian();
            _hasFilenames = (properties & GvmFlags.Filenames) != 0;
            _hasFormats = (properties & GvmFlags.Formats) != 0;
            _hasDimensions = (properties & GvmFlags.Dimensions) != 0;
            _hasGlobalIndexes = (properties & GvmFlags.GlobalIndexes) != 0;

            // Calculate the size of each entry in the entry table.
            _tableEntryLength = 2;
            if (_hasFilenames)
            {
                _tableEntryLength += 28;
            }
            if (_hasFormats)
            {
                _tableEntryLength += 2;
            }
            if (_hasDimensions)
            {
                _tableEntryLength += 2;
            }
            if (_hasGlobalIndexes)
            {
                _globalIndexOffset = _tableEntryLength;
                _tableEntryLength += 4;
            }

            // Get the number of entries in the archive.
            ushort numEntries = reader.ReadUInt16BigEndian();
            _entries = new List<GvmReaderEntry>(numEntries);

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                _stream.Position = _streamStart + entryOffset + 4;

                int length = reader.ReadInt32() + 8;

                // Get the name and the global index (both if applicable).
                string? name = null;
                if (_hasFilenames)
                {
                    name = reader.At(_streamStart + headerOffset + 2, x => x.ReadString(28)) + ".gvr";
                }

                uint? globalIndex = null;
                if (_hasGlobalIndexes)
                {
                    globalIndex = reader.At(_streamStart + headerOffset + _globalIndexOffset, x => x.ReadUInt32());
                }

                headerOffset += _tableEntryLength;

                // Some Billy Hatcher GVMs have an oddity where the reported length of the last texture is
                // 16 bytes more than what it actually is. When this occurs, we will correct the reported length.
                int reportedLength = length;
                if (_streamStart + entryOffset + length > source.Length)
                {
                    length = (int)(source.Length - (_streamStart + entryOffset));
                }

                // Add this entry to the entry collection.
                _entries.Add(new GvmReaderEntry(_stream, _streamStart + entryOffset, length, name, globalIndex, reportedLength));

                entryOffset += length;
            }

            // We're done reading this archive. Seek to the end of the stream.
            source.Seek(0, SeekOrigin.End);
        }

        /// <summary>
        /// Returns if the data in <paramref name="source"/> resembles an archive of this format.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns><see langword="true"/> if the data resembles an archive of this format, <see langword="false"/> otherwise.</returns>
        public static bool IsFormat(Stream source)
        {
            long startPosition = source.Position;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            return source.Length - startPosition > 12
                && reader.At(startPosition, x => x.ReadBytes(GvmConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(GvmConstants.MagicCode);
        }
    }
}
