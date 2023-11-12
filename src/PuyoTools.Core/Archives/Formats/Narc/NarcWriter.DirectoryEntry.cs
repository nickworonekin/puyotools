using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    public partial class NarcWriter
    {
        private class DirectoryEntry : Entry
        {
            public DirectoryEntry? Parent;
            public int NameEntryOffset;
            public ushort FirstFileIndex;
            public readonly List<Entry> Entries = new();
            public readonly Dictionary<string, DirectoryEntry> Directories = new(StringComparer.OrdinalIgnoreCase);
        }
    }
}
