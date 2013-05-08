using System;

namespace VrSharp.GvrTexture
{
    // Gvr Pixel Formats
    public enum GvrPixelFormat : byte
    {
        IntensityA8 = 0x00,
        Rgb565      = 0x01,
        Rgb5a3      = 0x02,
        Unknown     = 0xFF,
    }

    // Gvr Data Formats
    public enum GvrDataFormat : byte
    {
        Intensity4  = 0x00,
        Intensity8  = 0x01,
        IntensityA4 = 0x02,
        IntensityA8 = 0x03,
        Rgb565      = 0x04,
        Rgb5a3      = 0x05,
        Argb8888    = 0x06,
        Index4      = 0x08,
        Index8      = 0x09,
        Dxt1        = 0x0E,
        Unknown     = 0xFF,
    }

    // Gvr Data Flags
    [Flags]
    public enum GvrDataFlags : byte
    {
        None         = 0x0,
        Mipmaps      = 0x1,
        ExternalClut = 0x2,
        InternalClut = 0x8,
        Clut         = ExternalClut | InternalClut,
    }

    public enum GvrGbixType
    {
        /// <summary>
        /// A magic code of "GBIX". This is generally used for textures in GameCube games.
        /// </summary>
        Gbix,

        /// <summary>
        /// A magic code of "GCIX". This is generally used for textures in Wii games.
        /// </summary>
        Gcix,
    }

    public static class GvrCodecList
    {
        // Gets the pixel codec
        public static GvrPixelCodec GetPixelCodec(GvrPixelFormat format)
        {
            switch (format)
            {
                case GvrPixelFormat.IntensityA8:
                    return new GvrPixelCodec.IntensityA8();
                case GvrPixelFormat.Rgb565:
                    return new GvrPixelCodec.Rgb565();
                case GvrPixelFormat.Rgb5a3:
                    return new GvrPixelCodec.Rgb5a3();
            }

            return null;
        }

        // Gets the data codec
        public static GvrDataCodec GetDataCodec(GvrDataFormat format)
        {
            switch (format)
            {
                case GvrDataFormat.Intensity4:
                    return new GvrDataCodec.Intensity4();
                case GvrDataFormat.Intensity8:
                    return new GvrDataCodec.Intensity8();
                case GvrDataFormat.IntensityA4:
                    return new GvrDataCodec.IntensityA4();
                case GvrDataFormat.IntensityA8:
                    return new GvrDataCodec.IntensityA8();
                case GvrDataFormat.Rgb565:
                    return new GvrDataCodec.Rgb565();
                case GvrDataFormat.Rgb5a3:
                    return new GvrDataCodec.Rgb5a3();
                case GvrDataFormat.Argb8888:
                    return new GvrDataCodec.Argb8888();
                case GvrDataFormat.Index4:
                    return new GvrDataCodec.Index4();
                case GvrDataFormat.Index8:
                    return new GvrDataCodec.Index8();
                case GvrDataFormat.Dxt1:
                    return new GvrDataCodec.Dxt1();
            }

            return null;
        }
    }
}