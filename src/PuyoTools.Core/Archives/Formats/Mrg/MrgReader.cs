using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncodingExtensions = PuyoTools.Core.EncodingExtensions;

namespace PuyoTools.Archives.Formats.Mrg
{
    /// <summary>
    /// Archive reader for MRG archives used in Puyo Puyo Fever 2.
    /// </summary>
    public class MrgReader : ArchiveReader<ArchiveReaderEntry>
    {
        public MrgReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a Puyo Puyo Fever 2-based MRG archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            _stream.Position += 4;
            int numEntries = reader.ReadInt32();
            _entries = new List<ArchiveReaderEntry>(numEntries);

            _stream.Position += 8;

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                string extension = reader.ReadString(4, EncodingExtensions.ShiftJIS);
                int offset = reader.ReadInt32();
                int length = reader.ReadInt32();

                _stream.Position += 4;

                string name = reader.ReadString(32, EncodingExtensions.ShiftJIS);

                // Append the file extension to its name, if one is present.
                if (extension.Length != 0)
                {
                    name += $".{extension}";
                }

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
                && reader.At(startPosition, x => x.ReadBytes(MrgConstants.MagicCode.Length))
                    .AsSpan()
                    .SequenceEqual(MrgConstants.MagicCode);
        }
    }
}
