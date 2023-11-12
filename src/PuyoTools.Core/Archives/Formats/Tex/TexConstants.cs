using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Tex
{
    internal static class TexConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'T', (byte)'E', (byte)'X', (byte)'0' };
    }
}
