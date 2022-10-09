using PuyoTools.Archives.Formats.Afs;
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
    /// Archive reader for TXD archives used in the Sonic Storybook series.
    /// </summary>
    public class TxdStorybookReader : ArchiveReader<ArchiveReaderEntry>
    {
        public TxdStorybookReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a Sonic Storybook-based TXD archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            _stream.Position += 6;
            short numEntries = reader.ReadInt16BigEndian();
            _entries = new List<ArchiveReaderEntry>(numEntries);

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                int offset = reader.ReadInt32BigEndian();
                int length = reader.ReadInt32BigEndian();
                string name = Path.ChangeExtension(reader.ReadString(32), ".GVR");

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

            return source.Length - startPosition > 16
                && reader.At(startPosition, x => x.ReadBytes(TxdStorybookConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(TxdStorybookConstants.MagicCode);
        }
    }
}
