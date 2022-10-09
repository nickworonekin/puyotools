using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Storybook
{
    /// <summary>
    /// Archive reader for ONE archives used in the Sonic Storybook series.
    /// </summary>
    public class OneStorybookReader : ArchiveReader<OneStorybookReaderEntry>
    {
        public OneStorybookReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a Sonic Storybook-based ONE archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            int numEntries = reader.ReadInt32BigEndian();
            _entries = new List<OneStorybookReaderEntry>(numEntries);

            _stream.Position += 12;

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                string name = reader.ReadString(32);
                _stream.Position += 4; // File index isn't needed.
                int offset = reader.ReadInt32BigEndian();
                int length = reader.ReadInt32BigEndian();
                int uncompressedLength = reader.ReadInt32BigEndian();

                // Add this entry to the entry collection.
                _entries.Add(new OneStorybookReaderEntry(_stream, _streamStart + offset, length, uncompressedLength, name));
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

            if (!(source.Length - startPosition > 16
                && reader.At(startPosition + 0x4, x => x.ReadUInt32BigEndian()) == 0x10))
            {
                return false;
            }

            uint i = reader.At(startPosition + 0xC, x => x.ReadUInt32BigEndian());

            // 0 used for Sonic and the Secret Rings.
            // -1 used for Sonic and the Black Knight.
            if (i != 0xFFFFFFFF && i != 0x00000000)
            {
                return false;
            }

            return true;
        }
    }
}
