using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Svm
{
    internal static class SvmConstants
    {
        internal static ReadOnlySpan<byte> MagicCode => new[] { (byte)'P', (byte)'V', (byte)'M', (byte)'H' };

        internal static ReadOnlySpan<byte> GbixMagicCode => new[] { (byte)'G', (byte)'B', (byte)'I', (byte)'X' };

        internal static ReadOnlySpan<byte> PvrtMagicCode => new[] { (byte)'P', (byte)'V', (byte)'R', (byte)'T' };
    }
}
