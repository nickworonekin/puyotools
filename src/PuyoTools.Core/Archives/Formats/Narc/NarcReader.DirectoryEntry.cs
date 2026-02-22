using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    public partial class NarcReader
    {
        private record DirectoryEntry
        {
            public DirectoryEntry? Parent;
            public int NameEntryOffset;
            public int FirstFileIndex;
            public string Name = string.Empty;

            public string FullName => Parent?.Parent is not null
                ? $"{Parent.FullName}/{Name}"
                : Name;
        }
    }
}
