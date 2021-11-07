using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim
{
    public enum GimPixelFormat : ushort
    {
        Rgb565 = 0x0000,
        Argb1555 = 0x0001,
        Argb4444 = 0x0002,
        Argb8888 = 0x0003,
        Index4 = 0x0004,
        Index8 = 0x0005,
        Index16 = 0x0006,
        Index32 = 0x0007,
        Dxt1 = 0x0008,
        Dxt3 = 0x0009,
        Dxt5 = 0x000A,
        Dxt1Ext = 0x0108,
        Dxt3Ext = 0x0109,
        Dxt5Ext = 0x010A,
    }
}
