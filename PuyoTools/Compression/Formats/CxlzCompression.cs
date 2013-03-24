using System;
using System.IO;

namespace PuyoTools.Compression
{
    public class CxlzCompression : CompressionBase
    {
        public override void Decompress(byte[] source, long offset, Stream destination, int length)
        {
            // The CXLZ format is identical to LZ10 with the exception of "CXLZ" at the beginning of the file
            // As such, we'll just pass it to the LZ10 decompressor

            (new Lz10Compression()).Decompress(source, offset + 4, destination, length - 4);
        }

        public override void Compress(byte[] source, long offset, Stream destination, int length, string fname)
        {
            // The CXLZ format is identical to LZ10 with the exception of "CXLZ" at the beginning of the file
            // As such, we'll just write "CXLZ" to the destination then pass it off to the LZ10 compressor

            destination.Write(new byte[] { (byte)'C', (byte)'X', (byte)'L', (byte)'Z' }, 0, 4);

            (new Lz10Compression()).Compress(source, offset, destination, length, fname);
        }

        public override bool Is(Stream source, int length, string fname)
        {
            return (length > 8 && PTStream.Contains(source, 0, new byte[] { (byte)'C', (byte)'X', (byte)'L', (byte)'Z', 0x10 }));
        }

        public override bool CanCompress()
        {
            return false;
        }
    }
}