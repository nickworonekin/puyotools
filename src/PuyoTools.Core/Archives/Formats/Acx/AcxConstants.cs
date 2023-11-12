using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Acx
{
    internal static class AcxConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { 0, 0, 0, 0 };
    }
}
