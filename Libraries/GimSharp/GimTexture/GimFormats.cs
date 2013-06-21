using System;

namespace GimSharp
{
    // Gim Palette Formats
    public enum GimPaletteFormat : byte
    {
        Rgb565   = 0x00,
        Argb1555 = 0x01,
        Argb4444 = 0x02,
        Argb8888 = 0x03,
        Unknown  = 0xFF,
    }

    // Gim Data Formats
    public enum GimDataFormat : byte
    {
        Rgb565   = 0x00,
        Argb1555 = 0x01,
        Argb4444 = 0x02,
        Argb8888 = 0x03,
        Index4   = 0x04,
        Index8   = 0x05,
        Unknown  = 0xFF,
    }
}