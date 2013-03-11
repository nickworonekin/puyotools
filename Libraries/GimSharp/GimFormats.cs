/*
using System;

namespace GimSharp
{
    // Gim Pixel Formats
    public enum GimPixelFormat : byte
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
        Index16  = 0x06,
        Index32  = 0x07,
        Unknown  = 0xFF,
    }

    public static class GimCodecList
    {
        // Gets the pixel codec
        public static GimPixelCodec GetPixelCodec(GimPixelFormat format)
        {
            switch (format)
            {
                case GimPixelFormat.Rgb565:
                    return new GimPixelCodec.Rgb565();
                case GimPixelFormat.Argb1555:
                    return new GimPixelCodec.Argb1555();
                case GimPixelFormat.Argb4444:
                    return new GimPixelCodec.Argb4444();
                case GimPixelFormat.Argb8888:
                    return new GimPixelCodec.Argb8888();
            }

            return null;
        }

        // Gets the data codec
        public static GimDataCodec GetDataCodec(GimDataFormat format)
        {
            switch (format)
            {
                case GimDataFormat.Rgb565:
                    return new GimDataCodec.Rgb565();
                case GimDataFormat.Argb1555:
                    return new GimDataCodec.Argb1555();
                case GimDataFormat.Argb4444:
                    return new GimDataCodec.Argb4444();
                case GimDataFormat.Argb8888:
                    return new GimDataCodec.Argb8888();
                case GimDataFormat.Index4:
                    return new GimDataCodec.Index4();
                case GimDataFormat.Index8:
                    return new GimDataCodec.Index8();
                case GimDataFormat.Index16:
                    return new GimDataCodec.Index16();
                case GimDataFormat.Index32:
                    return new GimDataCodec.Index32();
            }

            return null;
        }
    }
}
*/