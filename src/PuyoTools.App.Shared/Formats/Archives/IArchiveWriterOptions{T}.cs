using System;
using System.Collections.Generic;
using System.Text;
using PuyoTools.Archives;

namespace PuyoTools.App.Formats.Archives
{
    interface IArchiveWriterOptions<T> : IArchiveWriterOptions
        where T : ArchiveWriter
    {
        void MapTo(T obj);

        void IArchiveWriterOptions.MapTo(ArchiveWriter obj) => MapTo((T)obj);

        void IMappable<ArchiveWriter>.MapTo(ArchiveWriter obj) => MapTo((T)obj);
    }
}
