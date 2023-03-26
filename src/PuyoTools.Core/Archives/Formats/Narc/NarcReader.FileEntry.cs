using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    public partial class NarcReader
    {
        private class FileEntry
        {
            public DirectoryEntry? Parent;
            public int Offset;
            public int Length;
            public string Name;
            public string FullName => Parent?.Parent is not null
                ? $"{Parent.FullName}/{Name}"
                : Name;
        }
    }
}
