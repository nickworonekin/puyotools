using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Spk
{
    internal static class SpkConstants
    {
        public static ReadOnlySpan<byte> MagicCode => "SND0"u8;
    }
}
