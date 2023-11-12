using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Spk
{
    internal static class SpkConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'S', (byte)'N', (byte)'D', (byte)'0' };
    }
}
