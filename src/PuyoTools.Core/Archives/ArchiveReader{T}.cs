using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives
{
    public abstract class ArchiveReader<T> : ArchiveReader
        where T : ArchiveReaderEntry
    {
        protected Stream _stream;
        protected long _streamStart;
        protected List<T> _entries;

        protected ArchiveReader(Stream source)
        {
            _stream = source;
            _streamStart = source.Position;
        }

        public override ReadOnlyCollection<T> Entries => _entries.AsReadOnly();
    }
}
