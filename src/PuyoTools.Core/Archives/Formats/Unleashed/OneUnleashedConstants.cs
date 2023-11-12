using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Unleashed
{
    internal static class OneUnleashedConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'o', (byte)'n', (byte)'e', (byte)'.' };
    }
}
