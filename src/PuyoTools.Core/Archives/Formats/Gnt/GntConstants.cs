using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gnt
{
    internal static class GntConstants
    {
        public static ReadOnlySpan<byte> PrimaryMagicCode => new byte[] { (byte)'N', (byte)'G', (byte)'I', (byte)'F' };

        public static ReadOnlySpan<byte> SecondaryMagicCode => new byte[] { (byte)'N', (byte)'G', (byte)'T', (byte)'L' };

        public static ReadOnlySpan<byte> Nof0MagicCode => new byte[] { (byte)'N', (byte)'O', (byte)'F', (byte)'0' };

        public static ReadOnlySpan<byte> NendMagicCode => new byte[] { (byte)'N', (byte)'E', (byte)'N', (byte)'D' };
    }
}
