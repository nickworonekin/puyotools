using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace PuyoTools.Modules.Texture
{
    public abstract class TextureBase : ModuleBase
    {
        public abstract string FileExtension { get; }
        public abstract string PaletteFileExtension { get; }

        /// <summary>
        /// After encoding a texture, returns if the texture needs an external palette file.
        /// The external palette file can be retrieved from PaletteStream.
        /// </summary>
        public virtual bool NeedsExternalPalette
        {
            get { return needsExternalPalette; }
        }
        protected bool needsExternalPalette = false;

        /// <summary>
        /// If decoding, sets the palette data for the texture.
        /// If encoding, this will hold the palette data if NeedsExternalPalette is true and after the texture has been encoded.
        /// </summary>
        public Stream PaletteStream { get; set; }

        public int PaletteLength = -1;

        //public abstract void Write(byte[] source, long offset, Stream destination, int length, string fname);
        public abstract void Write(Stream source, Stream destination, int length);

        #region Read Methods
        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public abstract void Read(Stream source, Stream destination, int length);

        /// <summary>
        /// Decodes a texture from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decompress.</param>
        /// <param name="destinationPath">File to decompress to.</param>
        public void Read(string sourcePath, string destinationPath)
        {
            // If we're reading from and writing to the same file, write the output to a temporary
            // file then move and replace the original file.
            if (sourcePath == destinationPath)
            {
                string tempPath = Path.GetTempFileName();

                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(tempPath))
                {
                    Read(source, destination, (int)source.Length);
                }

                File.Delete(sourcePath);
                File.Move(tempPath, destinationPath);
            }
            else
            {
                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(destinationPath))
                {
                    Read(source, destination, (int)source.Length);
                }
            }
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>>
        public void Read(Stream source, Stream destination)
        {
            Read(source, destination, (int)(source.Length - source.Position));
        }

        public void Read(Stream source, out Bitmap destination)
        {
            Read(source, out destination, (int)(source.Length - source.Position));
        }

        public void Read(Stream source, out Bitmap destination, int length)
        {
            MemoryStream destinationStream = new MemoryStream();
            Read(source, destinationStream, length);
            destination = new Bitmap(destinationStream);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        public void Read(byte[] source, out Bitmap destination)
        {
            Read(source, 0, out destination, source.Length);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Read(byte[] source, Stream destination)
        {
            Read(source, 0, destination, source.Length);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Read(byte[] source, int sourceIndex, out Bitmap destination, int length)
        {
            MemoryStream destinationStream = new MemoryStream();

            using (MemoryStream sourceStream = new MemoryStream())
            {
                sourceStream.Write(source, sourceIndex, length);
                sourceStream.Position = 0;

                Read(sourceStream, destinationStream, length);

                destination = new Bitmap(destinationStream);
            }
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Read(byte[] source, int sourceIndex, Stream destination, int length)
        {
            using (MemoryStream sourceStream = new MemoryStream())
            {
                sourceStream.Write(source, sourceIndex, length);
                sourceStream.Position = 0;

                Read(sourceStream, destination, length);
            }
        }
        #endregion
    }

    public class TextureNeedsPaletteException : Exception { }
}