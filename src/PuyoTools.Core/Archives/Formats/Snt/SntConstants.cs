using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Snt
{
    internal static class SntConstants
    {
        internal static ReadOnlySpan<byte> Ps2PrimaryMagicCode => new byte[] { (byte)'N', (byte)'S', (byte)'I', (byte)'F' };

        internal static ReadOnlySpan<byte> Ps2SecondaryMagicCode => new byte[] { (byte)'N', (byte)'S', (byte)'T', (byte)'L' };

        internal static ReadOnlySpan<byte> PspPrimaryMagicCode => new byte[] { (byte)'N', (byte)'U', (byte)'I', (byte)'F' };

        internal static ReadOnlySpan<byte> PspSecondaryMagicCode => new byte[] { (byte)'N', (byte)'U', (byte)'T', (byte)'L' };
    }
}
