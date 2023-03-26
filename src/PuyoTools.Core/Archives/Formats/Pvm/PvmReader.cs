using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Pvm
{
    /// <summary>
    /// Archive reader for PVM archives.
    /// </summary>
    public class PvmReader : ArchiveReader<PvmReaderEntry>
    {
        private bool _hasFilenames;
        private bool _hasFormats;
        private bool _hasDimensions;
        private bool _hasGlobalIndexes;
        private int _tableEntryLength;
        private int _globalIndexOffset;

        public PvmReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a PVM archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the offset of the first entry.
            _stream.Position += 4;
            int entryOffset = reader.ReadInt32() + 8;
            int headerOffset = 0xC;

            // Read the properties included in the archive.
            PvmFlags properties = (PvmFlags)reader.ReadUInt16();
            _hasFilenames = (properties & PvmFlags.Filenames) != 0;
            _hasFormats = (properties & PvmFlags.Formats) != 0;
            _hasDimensions = (properties & PvmFlags.Dimensions) != 0;
            _hasGlobalIndexes = (properties & PvmFlags.GlobalIndexes) != 0;

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
            _entries = new List<PvmReaderEntry>(numEntries);

            // Loop through all of the entries.
            // For this archive, we only increment the index when the entry being read is a texture.
            for (int i = 0; i < numEntries;)
            {
                _stream.Position = _streamStart + entryOffset;

                // Verify if the current chunk contains texture data. If not, skip over it.
                ReadOnlySpan<byte> chunkIdentifier = reader.ReadBytes(4);
                int chunkLength = reader.ReadInt32() + 8;

                if (!chunkIdentifier.SequenceEqual(PvmConstants.PvrtMagicCode))
                {
                    entryOffset += chunkLength;
                    continue;
                }

                // Get the name and the global index (both if applicable).
                string? name = null;
                if (_hasFilenames)
                {
                    name = reader.At(_streamStart + headerOffset + 2, x => x.ReadString(28)) + ".pvr";
                }

                uint? globalIndex = null;
                if (_hasGlobalIndexes)
                {
                    globalIndex = reader.At(_streamStart + headerOffset + _globalIndexOffset, x => x.ReadUInt32());
                }

                headerOffset += _tableEntryLength;

                // Add this entry to the entry collection.
                _entries.Add(new PvmReaderEntry(_stream, _streamStart + entryOffset, chunkLength, name, globalIndex));

                entryOffset += chunkLength;
                i++;
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
                && reader.At(startPosition, x => x.ReadBytes(PvmConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(PvmConstants.MagicCode)))
            {
                return false;
            }

            // Since PVMs and SVMs have identical headers, we need to check the data format of the first texture in the archive.
            // For PVRs, the data format will be < 0x60.
            int firstEntryOffset = reader.At(startPosition + 4, x => x.ReadInt32()) + 8;

            // Get the actual offset of the first texture.
            while (true)
            {
                if (remainingLength < firstEntryOffset + 16)
                {
                    return false;
                }

                ReadOnlySpan<byte> chunkIdentifier = reader.At(startPosition + firstEntryOffset, x => x.ReadBytes(4));
                int chunkLength = reader.At(startPosition + firstEntryOffset + 4, x => x.ReadInt32()) + 8;

                if (chunkIdentifier.SequenceEqual(PvmConstants.PvrtMagicCode))
                {
                    break;
                }

                firstEntryOffset += chunkLength;
            }

            byte dataFormat = reader.At(startPosition + firstEntryOffset + 0x9, x => x.ReadByte());

            return dataFormat < 0x60;
        }
    }
}
