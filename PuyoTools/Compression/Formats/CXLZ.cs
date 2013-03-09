using System;
using System.IO;

namespace PuyoTools2.Compression
{
    public class CXLZ : CompressionBase
    {
        public override bool Compress(byte[] source, long offset, int length, string fname, Stream destination)
        {
            // The CXLZ format is identical to LZ10 with the exception of "CXLZ" at the beginning of the file
            // As such, we'll just write "CXLZ" to the destination then pass it off to the LZ10 compressor

            destination.Write(new byte[] { (byte)'C', (byte)'X', (byte)'L', (byte)'Z' }, 0, 4);

            return (new LZ10()).Compress(source, offset, length, fname, destination);
        }

        public override bool Decompress(byte[] source, long offset, int length, Stream destination)
        {
            // The CXLZ format is identical to LZ10 with the exception of "CXLZ" at the beginning of the file
            // As such, we'll just pass it to the LZ10 decompressor

            return (new LZ10()).Decompress(source, offset + 4, length - 4, destination);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 8 && PTStream.Contains(source, 0, new byte[] { (byte)'C', (byte)'X', (byte)'L', (byte)'Z', 0x10 }));
        }

        public override bool CanCompress()
        {
            return true;
        }
    }
}