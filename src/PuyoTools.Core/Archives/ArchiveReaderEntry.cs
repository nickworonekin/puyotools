using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives
{
    public class ArchiveReaderEntry
    {
        protected Stream _stream;
        protected long _position;
        protected long _length;
        protected string _name;

        public ArchiveReaderEntry(Stream stream, long position, long length, string? name)
        {
            _stream = stream;
            _position = position;
            _length = length;
            _name = name ?? string.Empty;
        }

        /// <summary>
        /// Gets the size of the entry as stored in the archive.
        /// </summary>
        /// <remarks>This value may differ from the length of the stream returned from <see cref="Open"/>.</remarks>
        public long Length => _length;

        /// <summary>
        /// Gets the relative path of the entry in the archive.
        /// </summary>
        public string FullName => _name;

        /// <summary>
        /// Gets the file name of the entry in the archive.
        /// </summary>
        public string Name => Path.GetFileName(_name);

        /// <summary>
        /// Opens the entry from the archive for reading.
        /// </summary>
        /// <returns>The stream that represents the contents of the entry.</returns>
        public virtual Stream Open() => new StreamView(_stream, _position, _length);

        /// <summary>
        /// Extracts the entry to a file.
        /// </summary>
        /// <param name="destinationFileName">The path of the file to create from the contents of the entry.</param>
        /// <param name="overwrite"><see langword="true"/> to overwrite an existing file that has the same name as the destination file; otherwise, <see langword="false"/>.</param>
        public void ExtractToFile(string destinationFileName, bool overwrite = false)
        {
            using FileStream destination = new(destinationFileName, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write);

            Open().CopyTo(destination);
        }
    }
}
