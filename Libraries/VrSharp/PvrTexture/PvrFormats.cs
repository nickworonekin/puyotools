using System;

namespace VrSharp.PvrTexture
{
    // Pvr Pixel Formats
    public enum PvrPixelFormat : byte
    {
        Argb1555 = 0x00,
        Rgb565   = 0x01,
        Argb4444 = 0x02,
        Unknown  = 0xFF,
    }

    // Pvr Data Formats
    public enum PvrDataFormat : byte
    {
        SquareTwiddled           = 0x01,
        SquareTwiddledMipmaps    = 0x02,
        Vq                       = 0x03,
        VqMipmaps                = 0x04,
        Index4                   = 0x05,
        Index8                   = 0x07,
        Rectangle                = 0x09,
        RectangleTwiddled        = 0x0D,
        SmallVq                  = 0x10,
        SmallVqMipmaps           = 0x11,
        SquareTwiddledMipmapsDup = 0x12,
        Unknown                  = 0xFF,
    }

    // Pvr Compression Formats
    public enum PvrCompressionFormat
    {
        Rle,
        None,
    }

    public static class PvrCodecList
    {
        // Gets the pixel codec
        public static PvrPixelCodec GetPixelCodec(PvrPixelFormat format)
        {
            switch (format)
            {
                case PvrPixelFormat.Argb1555:
                    return new PvrPixelCodec.Argb1555();
                case PvrPixelFormat.Rgb565:
                    return new PvrPixelCodec.Rgb565();
                case PvrPixelFormat.Argb4444:
                    return new PvrPixelCodec.Argb4444();
            }

            return null;
        }

        // Gets the data codec
        public static PvrDataCodec GetDataCodec(PvrDataFormat format)
        {
            switch (format)
            {
                case PvrDataFormat.SquareTwiddled:
                    return new PvrDataCodec.SquareTwiddled();
                case PvrDataFormat.SquareTwiddledMipmaps:
                case PvrDataFormat.SquareTwiddledMipmapsDup:
                    return new PvrDataCodec.SquareTwiddledMipmaps();
                case PvrDataFormat.Vq:
                    return new PvrDataCodec.Vq();
                case PvrDataFormat.VqMipmaps:
                    return new PvrDataCodec.VqMipmaps();
                case PvrDataFormat.Index4:
                    return new PvrDataCodec.Index4();
                case PvrDataFormat.Index8:
                    return new PvrDataCodec.Index8();
                case PvrDataFormat.Rectangle:
                    return new PvrDataCodec.Rectangle();
                case PvrDataFormat.RectangleTwiddled:
                    return new PvrDataCodec.RectangleTwiddled();
                case PvrDataFormat.SmallVq:
                    return new PvrDataCodec.SmallVq();
                case PvrDataFormat.SmallVqMipmaps:
                    return new PvrDataCodec.SmallVqMipmaps();
            }

            return null;
        }

        // Gets the compression codec
        public static PvrCompressionCodec GetCompressionCodec(PvrCompressionFormat format)
        {
            switch (format)
            {
                case PvrCompressionFormat.Rle:
                    return new PvrCompressionCodec.Rle();
            }

            return null;
        }
    }
}