using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gvm
{
    internal static class GvmConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new[] { (byte)'G', (byte)'V', (byte)'M', (byte)'H' };

        public static ReadOnlySpan<byte> GbixMagicCode => new[] { (byte)'G', (byte)'B', (byte)'I', (byte)'X' };

        public static ReadOnlySpan<byte> GcixMagicCode => new[] { (byte)'G', (byte)'C', (byte)'I', (byte)'X' };
    }
}
