using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    public partial class NarcWriter
    {
        private abstract class Entry
        {
            public ushort Index;
            public string Name;
        }
    }
}
