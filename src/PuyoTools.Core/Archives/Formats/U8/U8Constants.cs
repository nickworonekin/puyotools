using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.U8
{
    internal static class U8Constants
    {
        public static ReadOnlySpan<byte> MagicCode => "U"u8 + "\xAA"u8 + "8-"u8;
    }
}
