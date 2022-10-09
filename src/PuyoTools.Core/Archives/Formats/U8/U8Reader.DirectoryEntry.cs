using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.U8
{
    public partial class U8Reader
    {
        private class DirectoryEntry
        {
            public DirectoryEntry? Parent;
            public string Name;
            public string FullName => Parent?.Parent is not null
                ? $"{Parent.FullName}/{Name}"
                : Name;
            public uint LastNodeIndex;
        }
    }
}
