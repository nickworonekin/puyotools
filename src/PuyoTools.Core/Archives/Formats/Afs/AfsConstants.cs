using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Afs
{
    internal class AfsConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'A', (byte)'F', (byte)'S', 0 };
    }
}
