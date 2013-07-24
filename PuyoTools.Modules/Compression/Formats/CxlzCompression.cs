using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public class CxlzCompression : Lz10Compression
    {
        // The CXLZ compression format is identical to the LZ10 compression format with the addition of
        // "CXLZ" at the beginning of the file.

        /// <summary>
        /// Name of the format.
        /// </summary>
        public override string Name
        {
            get { return "CXLZ"; }
        }

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public override void Decompress(Stream source, Stream destination)
        {
            source.Position += 4;

            base.Decompress(source, destination);
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
                throw new Exception("Source is too large. CXLZ compression can only compress files smaller than 16MB.");
            }

            destination.WriteByte((byte)'C');
            destination.WriteByte((byte)'X');
            destination.WriteByte((byte)'L');
            destination.WriteByte((byte)'Z');

            base.Compress(source, destination);
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