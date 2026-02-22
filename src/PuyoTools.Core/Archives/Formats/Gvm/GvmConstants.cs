using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gvm
{
    internal static class GvmConstants
    {
        public static ReadOnlySpan<byte> MagicCode => "GVMH"u8;

        public static ReadOnlySpan<byte> GbixMagicCode => "GBIX"u8;

        public static ReadOnlySpan<byte> GcixMagicCode => "GCIX"u8;
    }
}
