using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Unleashed
{
    /// <summary>
    /// Archive reader for ONE archives used in the PlayStation 2 and Wii versions of Sonic Unleashed.
    /// </summary>
    public class OneUnleashedReader : ArchiveReader<ArchiveReaderEntry>
    {
        public OneUnleashedReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a Sonic Unleashed-based ONE archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            _stream.Position += 4;
            int numEntries = reader.ReadInt32();
            _entries = new List<ArchiveReaderEntry>(numEntries);

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                string name = reader.ReadString(56);
                int offset = reader.ReadInt32();
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
            long startPosition = source.Position;

            using BinaryReader reader = new(source, Encoding.UTF8, true);

            return source.Length - startPosition > 8
                && reader.At(startPosition, x => x.ReadBytes(OneUnleashedConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(OneUnleashedConstants.MagicCode);
        }
    }
}
