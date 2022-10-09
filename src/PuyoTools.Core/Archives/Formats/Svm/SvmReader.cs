using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Svm
{
    /// <summary>
    /// Archive reader for SVM archives.
    /// </summary>
    public class SvmReader : ArchiveReader<SvmReaderEntry>
    {
        private bool _hasFilenames;
        private bool _hasFormats;
        private bool _hasDimensions;
        private bool _hasGlobalIndexes;
        private int _tableEntryLength;
        private int _globalIndexOffset;

        public SvmReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a SVM archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the offset of the first entry.
            _stream.Position += 4;
            int entryOffset = reader.ReadInt32() + 8;
            int headerOffset = 0xC;

            // Read the properties included in the archive.
            SvmFlags properties = (SvmFlags)reader.ReadUInt16();
            _hasFilenames = (properties & SvmFlags.Filenames) != 0;
            _hasFormats = (properties & SvmFlags.Formats) != 0;
            _hasDimensions = (properties & SvmFlags.Dimensions) != 0;
            _hasGlobalIndexes = (properties & SvmFlags.GlobalIndexes) != 0;

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
            ushort numEntries = reader.ReadUInt16();
            _entries = new List<SvmReaderEntry>(numEntries);

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                _stream.Position = _streamStart + entryOffset + 4;

                int length = reader.ReadInt32() + 8;

                // Get the name and the global index (both if applicable).
                string? name = null;
                if (_hasFilenames)
                {
                    name = reader.At(_streamStart + headerOffset + 2, x => x.ReadString(28)) + ".svr";
                }

                uint? globalIndex = null;
                if (_hasGlobalIndexes)
                {
                    globalIndex = reader.At(_streamStart + headerOffset + _globalIndexOffset, x => x.ReadUInt32());
                }

                headerOffset += _tableEntryLength;

                // Add this entry to the entry collection.
                _entries.Add(new SvmReaderEntry(_stream, _streamStart + entryOffset, length, name, globalIndex));

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
            long remainingLength = source.Length - startPosition;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            if (!(remainingLength > 12
                && reader.At(startPosition, x => x.ReadBytes(SvmConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(SvmConstants.MagicCode)))
            {
                return false;
            }

            // Since PVMs and SVMs have identical headers, we need to check the data format of the first texture in the archive.
            // For SVRs, the data format will be >= 0x60 and < 0x70.
            int firstEntryOffset = reader.At(startPosition + 4, x => x.ReadInt32()) + 8;

            byte dataFormat = reader.At(startPosition + firstEntryOffset + 0x9, x => x.ReadByte());

            return dataFormat is >= 0x60 and < 0x70;
        }
    }
}
