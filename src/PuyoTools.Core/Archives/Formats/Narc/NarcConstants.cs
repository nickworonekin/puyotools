using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    internal static class NarcConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'N', (byte)'A', (byte)'R', (byte)'C', 0xFE, 0xFF, 0x00, 0x01 };

        public static ReadOnlySpan<byte> FatbMagicCode => new byte[] { (byte)'B', (byte)'T', (byte)'A', (byte)'F' };

        public static ReadOnlySpan<byte> FntbMagicCode => new byte[] { (byte)'B', (byte)'T', (byte)'N', (byte)'F' };

        public static ReadOnlySpan<byte> FimgMagicCode => new byte[] { (byte)'G', (byte)'M', (byte)'I', (byte)'F' };
    }
}
