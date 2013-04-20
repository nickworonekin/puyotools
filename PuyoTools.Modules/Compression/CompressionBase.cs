using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public abstract class CompressionBase : ModuleBase
    {
        public abstract void Decompress(byte[] source, long offset, Stream destination, int length);
        public abstract void Compress(byte[] source, long offset, Stream destination, int length, string fname);

        #region Helper methods for Decompress
        public void Decompress(Stream source, Stream destination)
        {
            // Since no length is specified, the length will be size between the current offset
            // and the length of the stream.
            Decompress(source, destination, (int)(source.Length - source.Position));
        }

        public void Decompress(Stream source, Stream destination, int length)
        {
            // Read in the rest of the input stream
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            // Now we can decompress the data
            Decompress(buffer, 0, destination, length);
        }

        public void Decompress(byte[] source, Stream destination)
        {
            Decompress(source, 0, destination, source.Length);
        }
        #endregion

        #region Helper methods for Compress
        public void Compress(Stream source, Stream destination)
        {
            // Since no length is specified, the length will be size between the current offset
            // and the length of the stream.
            Compress(source, destination, (int)(source.Length - source.Position));
        }

        public void Compress(Stream source, Stream destination, int length)
        {
            Compress(source, destination, length, String.Empty);
        }

        public void Compress(Stream source, Stream destination, string fname)
        {
            Compress(source, destination, (int)(source.Length - source.Position), fname);
        }

        public void Compress(Stream source, Stream destination, int length, string fname)
        {
            // Read in the rest of the input stream
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            // Now we can decompress the data
            Compress(buffer, 0, destination, length, fname);
        }

        public void Compress(byte[] source, Stream destination)
        {
            Compress(source, 0, destination, source.Length, String.Empty);
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