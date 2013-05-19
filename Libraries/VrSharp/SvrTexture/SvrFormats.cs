using System;

namespace VrSharp.SvrTexture
{
    // Svr Pixel Formats
    public enum SvrPixelFormat : byte
    {
        Rgb5a3   = 0x08,
        Argb8888 = 0x09,
        Unknown  = 0xFF,
    }

    // Svr Data Formats
    public enum SvrDataFormat : byte
    {
        Rectangle        = 0x60,
        Index4ExtClut    = 0x62,
        Index8ExtClut    = 0x64,
        Index4RectRgb5a3 = 0x66,
        Index4SqrRgb5a3  = 0x67,
        Index4RectArgb8  = 0x68,
        Index4SqrArgb8   = 0x69,
        Index8RectRgb5a3 = 0x6A,
        Index8SqrRgb5a3  = 0x6B,
        Index8RectArgb8  = 0x6C,
        Index8SqrArgb8   = 0x6D,
        Unknown          = 0xFF,
    }

    /*
    public static class SvrCodecList
    {
        // Gets the pixel codec
        public static SvrPixelCodec GetPixelCodec(SvrPixelFormat format)
        {
            switch (format)
            {
                case SvrPixelFormat.Rgb5a3:
                    return new SvrPixelCodec.Rgb5a3();
                case SvrPixelFormat.Argb8888:
                    return new SvrPixelCodec.Argb8888();
            }

            return null;
        }

        // Gets the data codec
        public static SvrDataCodec GetDataCodec(SvrDataFormat format)
        {
            switch (format)
            {
                case SvrDataFormat.Rectangle:
                    return new SvrDataCodec.Rectangle();
                case SvrDataFormat.Index4ExtClut:
                    return new SvrDataCodec.Index4ExtClut();
                case SvrDataFormat.Index8ExtClut:
                    return new SvrDataCodec.Index8ExtClut();
                case SvrDataFormat.Index4RectRgb5a3:
                case SvrDataFormat.Index4SqrRgb5a3:
                case SvrDataFormat.Index4RectArgb8:
                case SvrDataFormat.Index4SqrArgb8:
                    return new SvrDataCodec.Index4();
                case SvrDataFormat.Index8RectRgb5a3:
                case SvrDataFormat.Index8SqrRgb5a3:
                case SvrDataFormat.Index8RectArgb8:
                case SvrDataFormat.Index8SqrArgb8:
                    return new SvrDataCodec.Index8();
            }

            return null;
        }
    }*/
}