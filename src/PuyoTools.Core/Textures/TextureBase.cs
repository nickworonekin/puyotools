using System;
using System.IO;

namespace PuyoTools.Core.Textures
{
    public abstract class TextureBase : ModuleBase
    {
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
        //public Stream PaletteStream { get; set; }

        #region Decode Methods
        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <remarks>A PNG will be written to the destination stream.</remarks>
        public abstract void Read(Stream source, Stream destination);

        /// <summary>
        /// Decodes a texture from part of a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <remarks>A PNG will be written to the destination stream.</remarks>
        public void Read(Stream source, Stream destination, int length)
        {
            Read(new StreamView(source, length), destination);
        }

        /// <summary>
        /// Decodes a texture from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decode.</param>
        /// <param name="destinationPath">File to decode to.</param>
        /// <remarks>A PNG will be written to the destination file.</remarks>
        public void Read(string sourcePath, string destinationPath)
        {
            // If we're reading from and writing to the same file, write the output to a temporary
            // file then move and replace the original file.
            if (sourcePath == destinationPath)
            {
                string tempPath = Path.GetTempFileName();

                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(tempPath))
                {
                    Read(source, destination);
                }

                File.Delete(sourcePath);
                File.Move(tempPath, destinationPath);
            }
            else
            {
                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(destinationPath))
                {
                    Read(source, destination);
                }
            }
        }

        /// <summary>
        /// Decodes a texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        public void Read(byte[] source, out byte[] destination)
        {
            Read(source, 0, out destination, source.Length);
        }

        /// <summary>
        /// Decodes a texture from part of a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">Offset of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Read(byte[] source, int offset, out byte[] destination, int length)
        {
            using (MemoryStream sourceStream = new MemoryStream(), destinationStream = new MemoryStream())
            {
                sourceStream.Write(source, offset, length);
                sourceStream.Position = 0;

                Read(sourceStream, destinationStream);

                destination = destinationStream.ToArray();
            }
        }

        /// <summary>
        /// Decodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A MemoryStream containing the decoded data.</returns>
        public MemoryStream Read(Stream source)
        {
            MemoryStream destination = new MemoryStream();
            Read(source, destination);
            destination.Position = 0;

            return destination;
        }

        /// <summary>
        /// Decodes a texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>A byte array containing the decoded data.</returns>
        public byte[] Read(byte[] source)
        {
            byte[] destination;
            Read(source, out destination);

            return destination;
        }
        #endregion

        #region Write Methods
        /// <summary>
        /// Encodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public abstract void Write(Stream source, Stream destination);

        /// <summary>
        /// Encodes a texture from part of a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public void Write(Stream source, Stream destination, int length)
        {
            Write(new StreamView(source, length), destination);
        }

        /// <summary>
        /// Encodes a texture from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decode.</param>
        /// <param name="destinationPath">File to decode to.</param>
        /// <remarks>A PNG will be written to the destination file.</remarks>
        public void Write(string sourcePath, string destinationPath)
        {
            // If we're reading from and writing to the same file, write the output to a temporary
            // file then move and replace the original file.
            if (sourcePath == destinationPath)
            {
                string tempPath = Path.GetTempFileName();

                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(tempPath))
                {
                    Write(source, destination);
                }

                File.Delete(sourcePath);
                File.Move(tempPath, destinationPath);
            }
            else
            {
                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(destinationPath))
                {
                    Write(source, destination);
                }
            }
        }

        /// <summary>
        /// Encodes a texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        public void Write(byte[] source, out byte[] destination)
        {
            Write(source, 0, out destination, source.Length);
        }

        /// <summary>
        /// Encodes a texture from part of a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">Offset of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Write(byte[] source, int offset, out byte[] destination, int length)
        {
            using (MemoryStream sourceStream = new MemoryStream(), destinationStream = new MemoryStream())
            {
                sourceStream.Write(source, offset, length);
                sourceStream.Position = 0;

                Write(sourceStream, destinationStream);

                destination = destinationStream.ToArray();
            }
        }

        /// <summary>
        /// Encodes a texture from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A MemoryStream containing the encoded data.</returns>
        public MemoryStream Write(Stream source)
        {
            MemoryStream destination = new MemoryStream();
            Write(source, destination);
            destination.Position = 0;

            return destination;
        }

        /// <summary>
        /// Encodes a texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>A byte array containing the encoded data.</returns>
        public byte[] Write(byte[] source)
        {
            byte[] destination;
            Write(source, out destination);

            return destination;
        }
        #endregion
    }

    public class TextureNeedsPaletteException : Exception { }
}