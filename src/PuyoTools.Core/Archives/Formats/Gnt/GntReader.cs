using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gnt
{
    /// <summary>
    /// Archive reader for GNT archives.
    /// </summary>
    public class GntReader : ArchiveReader<ArchiveReaderEntry>
    {
        public GntReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a GNT archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            _stream.Position += 48;
            int numEntries = reader.ReadInt32BigEndian();
            _entries = new List<ArchiveReaderEntry>(numEntries);

            _stream.Position += 8 + (numEntries * 20);

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                int length = reader.ReadInt32BigEndian();
                int offset = reader.ReadInt32BigEndian() + 32;

                // Add this entry to the entry collection.
                _entries.Add(new ArchiveReaderEntry(_stream, _streamStart + offset, length, string.Empty));
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

            return source.Length - startPosition > 36
                && reader.At(startPosition, x => x.ReadBytes(GntConstants.PrimaryMagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(GntConstants.PrimaryMagicCode)
                && reader.At(startPosition + 8, x => x.ReadInt32BigEndian()) == 1 // Check so we don't mis-identify GNO archives from the Sonic Storybook games as GNT archives
                && reader.At(startPosition + 32, x => x.ReadBytes(GntConstants.SecondaryMagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(GntConstants.SecondaryMagicCode);
        }
    }
}
