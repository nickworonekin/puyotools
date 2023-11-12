using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace PuyoTools.Core.Archives
{
    public abstract class ArchiveBase : ModuleBase
    {
        #region Open Methods
        /// <summary>
        /// Open an archive from a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <returns>An ArchiveReader object.</returns>
        /// <remarks>You must keep the stream open for the duration of the ArchiveReader.</remarks>
        public abstract LegacyArchiveReader Open(Stream source);

        /// <summary>
        /// Open an archive from part of a stream.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>An ArchiveReader object.</returns>
        /// <remarks>You must keep the stream open for the duration of the ArchiveReader.</remarks>
        public LegacyArchiveReader Open(Stream source, int length)
        {
            return Open(new StreamView(source, length));
        }

        /// <summary>
        /// Open an archive from a file.
        /// </summary>
        /// <param name="path">File to open.</param>
        /// <returns>An ArchiveReader object.</returns>
        public LegacyArchiveReader Open(string path)
        {
            return Open(File.OpenRead(path));
        }

        /// <summary>
        /// Open an archive from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>An ArchiveReader object.</returns>
        public LegacyArchiveReader Open(byte[] source)
        {
            return Open(source, 0, source.Length);
        }

        /// <summary>
        /// Open an archive from a byte array.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">Offset of the data in the source array.</param>
        /// <param name="length">Length of the data in the source array.</param>
        /// <returns>An ArchiveReader object.</returns>
        public LegacyArchiveReader Open(byte[] source, int offset, int length)
        {
            MemoryStream sourceStream = new MemoryStream();
            sourceStream.Write(source, 0, length);
            sourceStream.Position = 0;

            return Open(sourceStream);
        }
        #endregion

        #region Create Methods
        /// <summary>
        /// Create an archive.
        /// </summary>
        /// <param name="destination">The stream to write to.</param>
        /// <returns>An ArchiveWriter object.</returns>
        public abstract LegacyArchiveWriter Create(Stream destination);
        #endregion
    }
}