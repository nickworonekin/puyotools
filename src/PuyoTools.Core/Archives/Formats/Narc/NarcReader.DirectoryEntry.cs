using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    public partial class NarcReader
    {
        private class DirectoryEntry
        {
            public DirectoryEntry? Parent;
            public int NameEntryOffset;
            public int FirstFileIndex;
            public string Name;
            public string FullName => Parent?.Parent is not null
                ? $"{Parent.FullName}/{Name}"
                : Name;
        }
    }
}
