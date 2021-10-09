using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Compression
{
    public class CompCompression : CompressionBase
    {
        // The COMP compression format is identical to the LZ11 compression format with the addition of
        // "COMP" at the beginning of the file.

        private static readonly byte[] magicCode = { (byte)'C', (byte)'O', (byte)'M', (byte)'P', 0x11 };

        private static readonly Lz11Compression lz11Compression = new Lz11Compression();

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Decompress(Stream source, Stream destination)
        {
            source.Position += 4;

            lz11Compression.Decompress(source, destination);
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Compress(Stream source, Stream destination)
        {
            destination.Write(magicCode, 0, 4);

            lz11Compression.Compress(source, destination);
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