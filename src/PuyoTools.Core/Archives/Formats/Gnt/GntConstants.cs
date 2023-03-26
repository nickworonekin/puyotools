using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gnt
{
    internal static class GntConstants
    {
        internal static ReadOnlySpan<byte> PrimaryMagicCode => new byte[] { (byte)'N', (byte)'G', (byte)'I', (byte)'F' };

        internal static ReadOnlySpan<byte> SecondaryMagicCode => new byte[] { (byte)'N', (byte)'G', (byte)'T', (byte)'L' };
    }
}
