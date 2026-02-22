using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Tex
{
    internal static class TexConstants
    {
        public static ReadOnlySpan<byte> MagicCode => "TEX0"u8;
    }
}
