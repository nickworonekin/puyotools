using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Pvm
{
    internal static class PvmConstants
    {
        public static ReadOnlySpan<byte> MagicCode => "PVMH"u8;

        public static ReadOnlySpan<byte> GbixMagicCode => "GBIX"u8;

        public static ReadOnlySpan<byte> PvrtMagicCode => "PVRT"u8;
    }
}
