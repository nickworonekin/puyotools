using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class CxlzCompression : CompressionBase
    {
        public override string Name
        {
            get { return "CXLZ"; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public override void Decompress(Stream source, Stream destination, int length)
        {
            // The CXLZ format is identical to LZ10 with the exception of "CXLZ" at the beginning of the file
            // As such, we'll just pass it to the LZ10 decompressor

            source.Position += 4;
            (new Lz10Compression()).Decompress(source, destination, length - 4);
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public override void Compress(Stream source, Stream destination, int length)
        {
            // The CXLZ format is identical to LZ10 with the exception of "CXLZ" at the beginning of the file
            // As such, we'll just write "CXLZ" to the destination then pass it off to the LZ10 compressor

            destination.WriteByte((byte)'C');
            destination.WriteByte((byte)'X');
            destination.WriteByte((byte)'L');
            destination.WriteByte((byte)'Z');

            (new Lz10Compression()).Compress(source, destination, length);
        }

        /// <summary>
        /// Determines if the data is in the specified format.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="fname">Name of the file.</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 8 && PTStream.Contains(source, 0, new byte[] { (byte)'C', (byte)'X', (byte)'L', (byte)'Z', 0x10 }));
        }
    }
}