using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    internal static class NarcConstants
    {
        internal static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'N', (byte)'A', (byte)'R', (byte)'C', 0xFE, 0xFF, 0x00, 0x01 };
    }
}
