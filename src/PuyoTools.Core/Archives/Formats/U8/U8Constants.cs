using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.U8
{
    internal static class U8Constants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'U', 0xAA, (byte)'8', (byte)'-' };
    }
}
