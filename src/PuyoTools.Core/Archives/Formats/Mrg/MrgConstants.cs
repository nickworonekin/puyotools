using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Mrg
{
    internal static class MrgConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'M', (byte)'R', (byte)'G', (byte)'0' };
    }
}
