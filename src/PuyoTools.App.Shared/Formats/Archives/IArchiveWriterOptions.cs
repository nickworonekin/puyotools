using System;
using System.Collections.Generic;
using System.Text;
using PuyoTools.Archives;

namespace PuyoTools.App.Formats.Archives
{
    interface IArchiveWriterOptions : IMappable<ArchiveWriter>
    {
        new void MapTo(ArchiveWriter obj);
    }
}
