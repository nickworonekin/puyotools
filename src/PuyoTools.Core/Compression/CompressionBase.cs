using System;
using System.IO;

namespace PuyoTools.Core.Compression
{
    public abstract class CompressionBase : ModuleBase
    {
        #region Decompress Methods
        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public abstract void Decompress(Stream source, Stream destination);

        /// <summary>
        /// Decompress data from part of a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public void Decompress(Stream source, Stream destination, int length)
        {
            Decompress(new StreamView(source, length), destination);
        }

        /// <summary>
        /// Decompress data from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decompress.</param>
        /// <param name="destinationPath">File to decompress to.</param>
        public void Decompress(string sourcePath, string destinationPath)
        {
            // If we're reading from and writing to the same file, write the output to a temporary
            // file then move and replace the original file.
            if (sourcePath == destinationPath)
            {
                string tempPath = Path.GetTempFileName();

                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(tempPath))
                {
                    Decompress(source, destination);
                }

                File.Delete(sourcePath);
                File.Move(tempPath, destinationPath);
            }
            else
            {
                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(destinationPath))
                {
                    Decompress(source, destination);
                }
            }
        }

        /// <summary>
        /// Decompress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        public void Decompress(byte[] source, out byte[] destination)
        {
            Decompress(source, 0, out destination, source.Length);
        }

        /// <summary>
        /// Decompress data from part of a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">Offset of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Decompress(byte[] source, int offset, out byte[] destination, int length)
        {
            using (MemoryStream sourceStream = new MemoryStream(), destinationStream = new MemoryStream())
            {
                sourceStream.Write(source, offset, length);
                sourceStream.Position = 0;

                Decompress(sourceStream, destinationStream, length);

                destination = destinationStream.ToArray();
            }
        }

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A MemoryStream containing the decompressed data.</returns>
        public MemoryStream Decompress(Stream source)
        {
            MemoryStream destination = new MemoryStream();
            Decompress(source, destination);
            destination.Position = 0;

            return destination;
        }

        /// <summary>
        /// Decompress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>A byte array containing the decompressed data.</returns>
        public byte[] Decompress(byte[] source)
        {
            byte[] destination;
            Decompress(source, out destination);

            return destination;
        }
        #endregion

        #region Compress Methods
        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public abstract void Compress(Stream source, Stream destination);

        /// <summary>
        /// Compress data from part of a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public void Compress(Stream source, Stream destination, int length)
        {
            Compress(new StreamView(source, length), destination);
        }

        /// <summary>
        /// Compress data from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decompress.</param>
        /// <param name="destinationPath">File to decompress to.</param>
        public void Compress(string sourcePath, string destinationPath)
        {
            // If we're reading from and writing to the same file, write the output to a temporary
            // file then move and replace the original file.
            if (sourcePath == destinationPath)
            {
                string tempPath = Path.GetTempFileName();

                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(tempPath))
                {
                    Compress(source, destination);
                }

                File.Delete(sourcePath);
                File.Move(tempPath, destinationPath);
            }
            else
            {
                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(destinationPath))
                {
                    Compress(source, destination);
                }
            }
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        public void Compress(byte[] source, out byte[] destination)
        {
            Compress(source, 0, out destination, source.Length);
        }

        /// <summary>
        /// Compress data from part of a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">Offset of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Compress(byte[] source, int offset, out byte[] destination, int length)
        {
            using (MemoryStream sourceStream = new MemoryStream(), destinationStream = new MemoryStream())
            {
                sourceStream.Write(source, offset, length);
                sourceStream.Position = 0;

                Compress(sourceStream, destinationStream, length);

                destination = destinationStream.ToArray();
            }
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>A MemoryStream containing the compressed data.</returns>
        public MemoryStream Compress(Stream source)
        {
            MemoryStream destination = new MemoryStream();
            Compress(source, destination);
            destination.Position = 0;

            return destination;
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>A byte array containing the compressed data.</returns>
        public byte[] Compress(byte[] source)
        {
            byte[] destination;
            Compress(source, out destination);

            return destination;
        }
        #endregion
    }
}