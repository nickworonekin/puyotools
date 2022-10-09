using PuyoTools.Archives.Formats.Acx;
using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Afs
{
    /// <summary>
    /// Archive reader for AFS archives.
    /// </summary>
    public class AfsReader : ArchiveReader<ArchiveReaderEntry>
    {
        public AfsReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble an AFS archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            _stream.Position += 4;
            int numEntries = reader.ReadInt32();
            _entries = new List<ArchiveReaderEntry>(numEntries);

            // Get the offset of the entry metadata (name and timestamp).
            // In V1 archives, it's stored after the last entry in the entry table.
            // In V2 archives, it's stored before the data of the first entry.
            int metadataOffset = reader.At(_streamStart + 8 + (numEntries * 8), x => x.ReadInt32());

            if (metadataOffset == 0)
            {
                int firstEntryOffset = reader.At(_streamStart + 8, x => x.ReadInt32());
                metadataOffset = reader.At(_streamStart + firstEntryOffset - 8, x => x.ReadInt32());
            }

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                int offset = reader.ReadInt32();
                int length = reader.ReadInt32();
                string name = reader.At(_streamStart + metadataOffset + (i * 48), x => x.ReadString(32));

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
            long startPosition = source.Position;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            return source.Length - startPosition > 8
                && reader.At(startPosition, x => x.ReadBytes(AfsConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(AfsConstants.MagicCode);
        }
    }
}
