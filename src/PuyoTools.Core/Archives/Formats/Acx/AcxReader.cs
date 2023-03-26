using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Acx
{
    /// <summary>
    /// Archive reader for ACX archives.
    /// </summary>
    public class AcxReader : ArchiveReader<ArchiveReaderEntry>
    {
        public AcxReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble an ACX archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            _stream.Position += 4;
            int numEntries = reader.ReadInt32BigEndian();
            _entries = new List<ArchiveReaderEntry>(numEntries);

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                int offset = reader.ReadInt32BigEndian();
                int length = reader.ReadInt32BigEndian();

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
            long remainingLength = source.Length - startPosition;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            // Verify the magic code.
            if (!(remainingLength > 12
                && reader.At(startPosition, x => x.ReadBytes(AcxConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(AcxConstants.MagicCode)))
            {
                return false;
            }

            // Since ACX archives don't have an easy way to identify them outside of their file extension,
            // we'll check to see if the position of the first file is where it's expected to be.
            int numEntries = reader.At(startPosition + 4, x => x.ReadInt32BigEndian());
            long actualOffset = reader.At(startPosition + 8, x => x.ReadInt32BigEndian());
            long expectedOffset = 8 + (numEntries * 8);

            // Verify the offset of the first file.
            if (remainingLength > expectedOffset
                && (actualOffset == expectedOffset
                    || actualOffset % 2048 == 0))
            {
                return true;
            }

            return false;
        }
    }
}
