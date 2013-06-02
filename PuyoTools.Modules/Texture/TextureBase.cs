using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace PuyoTools.Modules.Texture
{
    public abstract class TextureBase : ModuleBase
    {
        public abstract string FileExtension { get; }
        public abstract string PaletteFileExtension { get; }

        public abstract void Write(byte[] source, long offset, Stream destination, int length, string fname);

        #region Read Methods
        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="settings">Settings to use when decoding.</param>
        public abstract void Read(Stream source, Stream destination, int length, TextureReaderSettings settings);

        /// <summary>
        /// Decodes a texture from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decompress.</param>
        /// <param name="destinationPath">File to decompress to.</param>
        public void Read(string sourcePath, string destinationPath)
        {
            Read(sourcePath, destinationPath, null);
        }

        /// <summary>
        /// Decodes a texture from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decompress.</param>
        /// <param name="destinationPath">File to decompress to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Read(string sourcePath, string destinationPath, TextureReaderSettings settings)
        {
            // If we're reading from and writing to the same file, write the output to a temporary
            // file then move and replace the original file.
            if (sourcePath == destinationPath)
            {
                string tempPath = Path.GetTempFileName();

                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(tempPath))
                {
                    Read(source, destination, (int)source.Length, settings);
                }

                File.Delete(sourcePath);
                File.Move(tempPath, destinationPath);
            }
            else
            {
                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(destinationPath))
                {
                    Read(source, destination, (int)source.Length, settings);
                }
            }
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public void Read(Stream source, Stream destination)
        {
            Read(source, destination, null);
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Read(Stream source, Stream destination, TextureReaderSettings settings)
        {
            Read(source, destination, (int)(source.Length - source.Position), settings);
        }

        public void Read(Stream source, out Bitmap destination)
        {
            Read(source, out destination, null);
        }

        public void Read(Stream source, out Bitmap destination, TextureReaderSettings settings)
        {
            Read(source, out destination, (int)(source.Length - source.Position), settings);
        }

        public void Read(Stream source, out Bitmap destination, int length)
        {
            Read(source, out destination, length, null);
        }

        public void Read(Stream source, out Bitmap destination, int length, TextureReaderSettings settings)
        {
            MemoryStream destinationStream = new MemoryStream();
            Read(source, destinationStream, length, settings);
            destination = new Bitmap(destinationStream);
        }

        /// <summary>
        /// Decodes a texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        public void Read(byte[] source, out Bitmap destination)
        {
            Read(source, out destination, null);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Read(byte[] source, out Bitmap destination, TextureReaderSettings settings)
        {
            Read(source, 0, out destination, source.Length, settings);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">The stream to write to.</param>
        public void Read(byte[] source, Stream destination)
        {
            Read(source, destination, null);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Read(byte[] source, Stream destination, TextureReaderSettings settings)
        {
            Read(source, 0, destination, source.Length, settings);
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
            Read(source, sourceIndex, out destination, length, null);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Read(byte[] source, int sourceIndex, out Bitmap destination, int length, TextureReaderSettings settings)
        {
            MemoryStream destinationStream = new MemoryStream();

            using (MemoryStream sourceStream = new MemoryStream())
            {
                sourceStream.Write(source, sourceIndex, length);
                sourceStream.Position = 0;

                Read(sourceStream, destinationStream, length, settings);

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
        public void Read(byte[] source, int sourceIndex, Stream destination, int length)
        {
            Read(source, sourceIndex, destination, length, null);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Read(byte[] source, int sourceIndex, Stream destination, int length, TextureReaderSettings settings)
        {
            using (MemoryStream sourceStream = new MemoryStream())
            {
                sourceStream.Write(source, sourceIndex, length);
                sourceStream.Position = 0;

                Read(sourceStream, destination, length, settings);
            }
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public void Read(Stream source, Stream destination, int length)
        {
            Read(source, destination, length, null);
        }
        #endregion
    }

    public class TextureReaderSettings
    {
        public Stream PaletteStream = null;
        public int PaletteLength = -1;
    }

    public class TextureWriterSettings
    {
        public string DestinationDirectory = String.Empty;
        public string DestinationFileName = String.Empty;
    }

    public class TextureNeedsPaletteException : Exception { }
}