using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr
{
    public enum PvrPixelFormat : byte
    {
        Argb1555 = 0x00,
        Rgb565   = 0x01,
        Argb4444 = 0x02,
        Yuv422   = 0x03,
        Bump     = 0x04,
        Argb8888 = 0x06,
    }
}
