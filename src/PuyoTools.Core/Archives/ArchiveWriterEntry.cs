using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PuyoTools.Archives
{
    public class ArchiveWriterEntry
    {
        protected Stream _source;
        protected long _length;
        protected string? _name;
        protected bool _leaveOpen;

        public ArchiveWriterEntry(Stream source, string? name = null, bool leaveOpen = false)
        {
            _source = source;
            _length = source.Length - source.Position;
            _name = name;
            _leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Gets the size of the entry.
        /// </summary>
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
        /// Writes the contents of this entry to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to which the contents of this entry will be written.</param>
        /// <remarks>
        /// If this entry was constructed with leaveOpen set to <see langword="false"/>, the entry's source stream will be closed
        /// after the contents are written.
        /// </remarks>
        public virtual void WriteTo(Stream destination)
        {
            _source.CopyTo(destination);

            if (!_leaveOpen)
            {
                _source.Close();
            }
        }
    }
}
