using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace PuyoTools.Texture
{
    public abstract class TextureBase
    {
        public abstract void Read(byte[] source, long offset, out Bitmap destination, int length);
        public abstract void Write(byte[] source, long offset, Stream destination, int length, string fname);
        public abstract bool Is(Stream source, int length, string fname);

        // It's assumed that this format can be read.
        // So, we're only going to bother seeing if this format can be written to.
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

        #region Helper methods for ReadWithPalette
        public void ReadWithPalette(Stream source, Stream palette, Stream destination)
        {
            // Since no length is specified, the length will be size between the current offset
            // and the length of the stream.
            ReadWithPalette(source, palette, destination, (int)(source.Length - source.Position), (int)(palette.Length - palette.Position));
        }

        public void ReadWithPalette(Stream source, Stream palette, out Bitmap destination)
        {
            ReadWithPalette(source, palette, out destination, (int)(source.Length - source.Position), (int)(palette.Length - palette.Position));
        }

        public void ReadWithPalette(Stream source, Stream palette, Stream destination, int length, int paletteLength)
        {
            // Read in the rest of the input stream
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            // Read in the palette stream
            byte[] paletteBuffer = new byte[paletteLength];
            palette.Read(paletteBuffer, 0, paletteLength);

            // Now we can decompress the data
            ReadWithPalette(buffer, 0, paletteBuffer, 0, destination, length, paletteLength);
        }

        public void ReadWithPalette(Stream source, Stream palette, out Bitmap destination, int length, int paletteLength)
        {
            // Read in the rest of the input stream
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            // Read in the palette stream
            byte[] paletteBuffer = new byte[paletteLength];
            palette.Read(paletteBuffer, 0, paletteLength);

            // Now we can decompress the data
            ReadWithPalette(buffer, 0, paletteBuffer, 0, out destination, length, paletteLength);
        }

        public void ReadWithPalette(byte[] source, byte[] palette, Stream destination)
        {
            ReadWithPalette(source, 0, palette, 0, destination, source.Length, palette.Length);
        }

        public void ReadWithPalette(byte[] source, byte[] palette, out Bitmap destination)
        {
            ReadWithPalette(source, 0, palette, 0, out destination, source.Length, palette.Length);
        }

        public void ReadWithPalette(byte[] source, long offset, byte[] palette, long paletteOffset, Stream destination, int length, int paletteLength)
        {
            Bitmap texture;
            ReadWithPalette(source, offset, palette, paletteOffset, out texture, length, paletteLength);
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