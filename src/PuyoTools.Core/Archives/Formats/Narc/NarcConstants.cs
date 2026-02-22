using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    internal static class NarcConstants
    {
        public static ReadOnlySpan<byte> MagicCode => "NARC\xFE\xFF\x00\x01"u8;

        public static ReadOnlySpan<byte> FatbMagicCode => "BTAF"u8;

        public static ReadOnlySpan<byte> FntbMagicCode => "BTNF"u8;

        public static ReadOnlySpan<byte> FimgMagicCode => "GMIF"u8;
    }
}
