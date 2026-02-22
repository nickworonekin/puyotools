using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gnt
{
    internal static class GntConstants
    {
        public static ReadOnlySpan<byte> PrimaryMagicCode => "NGIF"u8;

        public static ReadOnlySpan<byte> SecondaryMagicCode => "NGTL"u8;

        public static ReadOnlySpan<byte> Nof0MagicCode => "NOF0"u8;

        public static ReadOnlySpan<byte> NendMagicCode => "NEND"u8;
    }
}
