using System;
using System.IO;

namespace PuyoTools.Modules.Compression
{
    public abstract class CompressionBase : ModuleBase
    {
        public abstract void Decompress(byte[] source, long offset, Stream destination, int length);
        public abstract void Compress(byte[] source, long offset, Stream destination, int length, string fname);

        #region Decompress Methods
        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        public void Decompress(Stream source, Stream destination, int length)
        {
            // Temporary!!!
            // This will eventually become the abstract method.
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            Decompress(buffer, 0L, destination, length);
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
                    Decompress(source, destination, (int)source.Length);
                }

                File.Delete(sourcePath);
                File.Move(tempPath, destinationPath);
            }
            else
            {
                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(destinationPath))
                {
                    Decompress(source, destination, (int)source.Length);
                }
            }
        }

        /// <summary>
        /// Decompress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public void Decompress(Stream source, Stream destination)
        {
            Decompress(source, destination, (int)(source.Length - source.Position));
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
        /// Decompress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">The stream to write to.</param>
        public void Decompress(byte[] source, Stream destination)
        {
            Decompress(source, 0, destination, source.Length);
        }

        /// <summary>
        /// Decompress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Decompress(byte[] source, int sourceIndex, out byte[] destination, int length)
        {
            using (MemoryStream sourceStream = new MemoryStream(), destinationStream = new MemoryStream())
            {
                sourceStream.Write(source, sourceIndex, length);
                sourceStream.Position = 0;

                Decompress(sourceStream, destinationStream, length);

                destination = destinationStream.ToArray();
            }
        }

        /// <summary>
        /// Decompress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Decompress(byte[] source, int sourceIndex, Stream destination, int length)
        {
            using (MemoryStream sourceStream = new MemoryStream())
            {
                sourceStream.Write(source, sourceIndex, length);
                sourceStream.Position = 0;

                Decompress(sourceStream, destination, length);
            }
        }
        #endregion

        #region Compress Methods
        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Compress(Stream source, Stream destination, int length, ModuleWriterSettings settings)
        {
            // Temporary!!!
            // This will eventually become the abstract method.
            byte[] buffer = new byte[length];
            source.Read(buffer, 0, length);

            Compress(buffer, 0L, destination, length, String.Empty);
        }

        /// <summary>
        /// Compress data from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decompress.</param>
        /// <param name="destinationPath">File to decompress to.</param>
        public void Compress(string sourcePath, string destinationPath)
        {
            Compress(sourcePath, destinationPath, null);
        }

        /// <summary>
        /// Compress data from a file. This method can read from and write to the same file.
        /// </summary>
        /// <param name="sourcePath">File to decompress.</param>
        /// <param name="destinationPath">File to decompress to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Compress(string sourcePath, string destinationPath, ModuleWriterSettings settings)
        {
            // If we're reading from and writing to the same file, write the output to a temporary
            // file then move and replace the original file.
            if (sourcePath == destinationPath)
            {
                string tempPath = Path.GetTempFileName();

                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(tempPath))
                {
                    Compress(source, destination, (int)source.Length, settings);
                }

                File.Delete(sourcePath);
                File.Move(tempPath, destinationPath);
            }
            else
            {
                using (FileStream source = File.OpenRead(sourcePath), destination = File.Create(destinationPath))
                {
                    Compress(source, destination, (int)source.Length, settings);
                }
            }
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        public void Compress(Stream source, Stream destination)
        {
            Compress(source, destination, null);
        }

        /// <summary>
        /// Compress data from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Compress(Stream source, Stream destination, ModuleWriterSettings settings)
        {
            Compress(source, destination, (int)(source.Length - source.Position), settings);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        public void Compress(byte[] source, out byte[] destination)
        {
            Compress(source, out destination, null);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Compress(byte[] source, out byte[] destination, ModuleWriterSettings settings)
        {
            Compress(source, 0, out destination, source.Length, settings);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">The stream to write to.</param>
        public void Compress(byte[] source, Stream destination)
        {
            Compress(source, destination, null);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Compress(byte[] source, Stream destination, ModuleWriterSettings settings)
        {
            Compress(source, 0, destination, source.Length, settings);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Compress(byte[] source, int sourceIndex, out byte[] destination, int length)
        {
            Compress(source, sourceIndex, out destination, length, null);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">Byte array to write the data to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Compress(byte[] source, int sourceIndex, out byte[] destination, int length, ModuleWriterSettings settings)
        {
            using (MemoryStream sourceStream = new MemoryStream(), destinationStream = new MemoryStream())
            {
                sourceStream.Write(source, sourceIndex, length);
                sourceStream.Position = 0;

                Compress(sourceStream, destinationStream, length, settings);

                destination = destinationStream.ToArray();
            }
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        public void Compress(byte[] source, int sourceIndex, Stream destination, int length)
        {
            Compress(source, sourceIndex, destination, length, null);
        }

        /// <summary>
        /// Compress data from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index of the data in the source array.</param>
        /// <param name="destination">The stream to write to.</param>
        /// <param name="length">Length of the data in the source array.</param>
        /// <param name="settings">Settings to use when compressing.</param>
        public void Compress(byte[] source, int sourceIndex, Stream destination, int length, ModuleWriterSettings settings)
        {
            using (MemoryStream sourceStream = new MemoryStream())
            {
                sourceStream.Write(source, sourceIndex, length);
                sourceStream.Position = 0;

                Compress(sourceStream, destination, length, settings);
            }
        }
        #endregion
    }
}