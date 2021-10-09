using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Compression
{
    public class CxlzCompression : CompressionBase// : Lz10Compression
    {
        // The CXLZ compression format is identical to the LZ10 compression format with the addition of
        // "CXLZ" at the beginning of the file.

        private static readonly byte[] magicCode = { (byte)'C', (byte)'X', (byte)'L', (byte)'Z', 0x10 };

        private static readonly Lz10Compression lz10Compression = new Lz10Compression();

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Decompress(Stream source, Stream destination)
        {
            source.Position += 4;

            lz10Compression.Decompress(source, destination);
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Compress(Stream source, Stream destination)
        {
            // CXLZ compression can only handle files smaller than 16MB
            if (source.Length - source.Position > 0xFFFFFF)
            {
                throw new Exception($"CXLZ compression can't be used to compress files larger than {0xFFFFFF:N0} bytes.");
            }

            destination.Write(magicCode, 0, 4);

            lz10Compression.Compress(source, destination);
        }

        /// <summary>
        /// Returns if this codec can read the data in <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data to read.</param>
        /// <returns>True if the data can be read, false otherwise.</returns>
        public static bool Identify(Stream source)
        {
            var startPosition = source.Position;

            using (var reader = new BinaryReader(source, Encoding.UTF8, true))
            {
                return source.Length - startPosition > 8
                    && reader.At(startPosition, x => x.ReadBytes(magicCode.Length)).SequenceEqual(magicCode);
            }
        }
    }
}