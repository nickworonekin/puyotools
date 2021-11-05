using System;

namespace PuyoTools.Core.Textures.Gim
{
    // Gim Palette Formats
    public enum GimPaletteFormat : byte
    {
        Rgb565   = 0x00,
        Argb1555 = 0x01,
        Argb4444 = 0x02,
        Argb8888 = 0x03,
    }

    // Gim Data Formats
    public enum GimDataFormat : ushort
    {
        Rgb565   = 0x0000,
        Argb1555 = 0x0001,
        Argb4444 = 0x0002,
        Argb8888 = 0x0003,
        Index4   = 0x0004,
        Index8   = 0x0005,
        Index16  = 0x0006,
        Index32  = 0x0007,
        Dxt1     = 0x0008,
        Dxt3     = 0x0009,
        Dxt5     = 0x000A,
        Dxt1Ext  = 0x0108,
        Dxt3Ext  = 0x0109,
        Dxt5Ext  = 0x010A,
    }
}