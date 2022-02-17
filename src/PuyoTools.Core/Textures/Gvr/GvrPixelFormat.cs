using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gvr
{
    public enum GvrDataFormat : byte
    {
        Intensity4 = 0x00,
        Intensity8 = 0x01,
        IntensityA4 = 0x02,
        IntensityA8 = 0x03,
        Rgb565 = 0x04,
        Rgb5a3 = 0x05,
        Argb8888 = 0x06,
        Index4 = 0x08,
        Index8 = 0x09,
        Dxt1 = 0x0E,
    }
}
