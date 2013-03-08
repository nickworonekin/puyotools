using System;
using System.IO;

namespace PuyoTools2.Compression
{
    public abstract class CompressionBase
    {
        public abstract bool Compress(byte[] source, long offset, int length, string fname, Stream destination);
        public abstract bool Decompress(byte[] source, long offset, int length, Stream destination);

        public abstract bool Is(Stream source, int length, string fname);

        // It's assumed that this format can be read (in this case, decompressed).
        // So, we're only going to bother seeing if this format can be written to (compressed).
        public abstract bool CanCompress();

        #region Helper methods for Decompress
        public bool Decompress(Stream source, Stream destination)
        {
            // Since no length is specified, the length will be size between the current offset
            // and the length of the stream.
            return Decompress(source, (int)(source.Length - source.Position), destination);
        }

        public bool Decompress(Stream source, int length, Stream destination)
        {
            // Read in the rest of the input stream
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            // Now we can decompress the data
            return Decompress(buffer, 0, length, destination);
        }

        public bool Decompress(byte[] source, Stream destination)
        {
            return Decompress(source, 0, source.Length, destination);
        }
        #endregion

        #region Helper methods for Is
        public bool Is(Stream source, string fname)
        {
            // Since no length is specified, the length will be size between the current offset
            // and the length of the stream.
            return Is(source, (int)(source.Length - source.Position), fname);
        }

        public bool Is(byte[] source, string fname)
        {
            return Is(new MemoryStream(source), fname);
        }
        #endregion
    }
}