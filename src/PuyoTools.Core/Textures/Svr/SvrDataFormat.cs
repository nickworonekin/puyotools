using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Svr
{
    public enum SvrDataFormat : byte
    {
        Rectangle             = 0x60,
        Index4ExternalPalette = 0x62,
        Index8ExternalPalette = 0x64,
        Index4Rgb5a3Rectangle = 0x66,
        Index4Rgb5a3Square    = 0x67,
        Index4Argb8Rectangle  = 0x68,
        Index4Argb8Square     = 0x69,
        Index8Rgb5a3Rectangle = 0x6A,
        Index8Rgb5a3Square    = 0x6B,
        Index8Argb8Rectangle  = 0x6C,
        Index8Argb8Square     = 0x6D,
        Index4                = Index4Rgb5a3Rectangle,
        Index8                = Index8Rgb5a3Rectangle,
    }
}
