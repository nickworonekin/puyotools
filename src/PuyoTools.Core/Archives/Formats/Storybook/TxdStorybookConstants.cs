using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Storybook
{
    internal static class TxdStorybookConstants
    {
        public static ReadOnlySpan<byte> MagicCode => new byte[] { (byte)'T', (byte)'X', (byte)'A', (byte)'G' };
    }
}
