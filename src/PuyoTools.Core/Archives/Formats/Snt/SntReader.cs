using PuyoTools.Core;
using PuyoTools.Core.Textures.Gim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Snt
{
    /// <summary>
    /// Archive reader for SNT archives.
    /// </summary>
    public class SntReader : ArchiveReader<ArchiveReaderEntry>
    {
        public SntReader(Stream source) : base(source)
        {
            // Verify the data is in the archive's format.
            if (!IsFormat(_stream))
            {
                throw new InvalidFormatException("The contents of this stream does not resemble a SNT archive.");
            }

            using BinaryReader reader = new(_stream, Encoding.UTF8, true);

            // Get the number of entries in the archive.
            _stream.Position += 48;
            int numEntries = reader.ReadInt32();
            _entries = new List<ArchiveReaderEntry>(numEntries);

            _stream.Position += 8 + (numEntries * 20);

            // Loop through all of the entries.
            for (int i = 0; i < numEntries; i++)
            {
                // Read the entry information.
                int length = reader.ReadInt32();
                int offset = reader.ReadInt32() + 32;

                // Although entries in SNT archives do not have filenames,
                // GIM textures stored in them may contain filenames in their metadata.
                // We'll make an attempt to read & use those filenames.
                string name = GetFilename(_stream, _streamStart + offset, length);

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

            if (!(source.Length - startPosition > 36
                && reader.At(startPosition + 8, x => x.ReadInt32()) == 1))
            {
                return false;
            }

            ReadOnlySpan<byte> primaryMagicCode = reader.At(startPosition, x => x.ReadBytes(4));
            ReadOnlySpan<byte> secondaryMagicCode = reader.At(startPosition + 32, x => x.ReadBytes(4));

            // Verify PS2 magic code.
            if (primaryMagicCode.SequenceEqual(SntConstants.Ps2PrimaryMagicCode)
                && secondaryMagicCode.SequenceEqual(SntConstants.Ps2SecondaryMagicCode))
            {
                return true;
            }

            // Verify PSP magic code.
            if (primaryMagicCode.SequenceEqual(SntConstants.PspPrimaryMagicCode)
                && secondaryMagicCode.SequenceEqual(SntConstants.PspSecondaryMagicCode))
            {
                return true;
            }

            return false;
        }

        private static string GetFilename(Stream source, long position, long length)
        {
            string? filename = null;

            long currentPosition = source.Position;

            using (Stream stream = new StreamView(source, position, length))
            {
                if (GimTextureDecoder.Is(stream))
                {
                    GimTextureDecoder gimDecoder = new(stream);
                    filename = Path.ChangeExtension(gimDecoder.Metadata?.OriginalFilename, ".gim");
                }
            }

            source.Position = currentPosition;

            return filename ?? string.Empty;
        }
    }
}
