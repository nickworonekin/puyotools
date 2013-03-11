using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace PuyoTools2.Texture
{
    public abstract class TextureBase
    {
        public abstract void Read(byte[] source, long offset, out Bitmap destination, int length);
        public abstract void Write(byte[] source, long offset, Stream destination, int length, string fname);
        public abstract bool Is(Stream source, int length, string fname);

        // It's assumed that this format can be read (in this case, decompressed).
        // So, we're only going to bother seeing if this format can be written to (compressed).
        public abstract bool CanWrite();

        public virtual void ReadWithPalette(byte[] source, long offset, byte[] palette, long paletteOffset, out Bitmap destination, int length, int paletteLength)
        {
            Read(source, offset, out destination, length);
        }

        #region Helper methods for Read
        public void Read(Stream source, Stream destination)
        {
            // Since no length is specified, the length will be size between the current offset
            // and the length of the stream.
            Read(source, destination, (int)(source.Length - source.Position));
        }

        public void Read(Stream source, out Bitmap destination)
        {
            Read(source, out destination, (int)(source.Length - source.Position));
        }

        public void Read(Stream source, Stream destination, int length)
        {
            // Read in the rest of the input stream
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            // Now we can decompress the data
            Read(buffer, 0, destination, length);
        }

        public void Read(Stream source, out Bitmap destination, int length)
        {
            // Read in the rest of the input stream
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            // Now we can decompress the data
            Read(buffer, 0, out destination, length);
        }

        public void Read(byte[] source, Stream destination)
        {
            Read(source, 0, destination, source.Length);
        }

        public void Read(byte[] source, out Bitmap destination)
        {
            Read(source, 0, out destination, source.Length);
        }

        public void Read(byte[] source, long offset, Stream destination, int length)
        {
            Bitmap texture;
            Read(source, offset, out texture, length);
            texture.Save(destination, ImageFormat.Png);
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

    public class TextureWriterSettings
    {
    }

    public class TextureNeedsPalette : Exception { }
}