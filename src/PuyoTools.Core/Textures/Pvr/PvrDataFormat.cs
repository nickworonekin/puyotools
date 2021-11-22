using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Pvr
{
    public enum PvrDataFormat : byte
    {
        SquareTwiddled           = 0x01,
        SquareTwiddledMipmaps    = 0x02,
        Vq                       = 0x03,
        VqMipmaps                = 0x04,
        Index4                   = 0x05,
        Index4Mipmaps            = 0x06,
        Index8                   = 0x07,
        Index8Mipmaps            = 0x08,
        Rectangle                = 0x09,
        Stride                   = 0x0B,
        RectangleTwiddled        = 0x0D,
        SmallVq                  = 0x10,
        SmallVqMipmaps           = 0x11,
        SquareTwiddledMipmapsAlt = 0x12,
    }
}
