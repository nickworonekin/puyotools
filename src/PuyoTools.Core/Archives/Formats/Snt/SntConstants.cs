using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Snt
{
    internal static class SntConstants
    {
        public static ReadOnlySpan<byte> Ps2PrimaryMagicCode => "NSIF"u8;

        public static ReadOnlySpan<byte> Ps2SecondaryMagicCode => "NSTL"u8;

        public static ReadOnlySpan<byte> PspPrimaryMagicCode => "NUIF"u8;

        public static ReadOnlySpan<byte> PspSecondaryMagicCode => "NUTL"u8;

        public static ReadOnlySpan<byte> Nof0MagicCode => "NOF0"u8;

        public static ReadOnlySpan<byte> NendMagicCode => "NEND"u8;
    }
}
