using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    public partial class NarcWriter
    {
        private class FileEntry : Entry
        {
            public ArchiveWriterEntry Entry;
            public int Offset;
            public int Length;
        }
    }
}
