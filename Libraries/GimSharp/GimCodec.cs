using System;
using System.Collections.Generic;

namespace GimSharp
{
    // GIM Palette Formats
    public enum GimPixelFormat : byte
    {
        Rgb565   = 0x00,
        Argb1555 = 0x01,
        Argb4444 = 0x02,
        Argb8888 = 0x03,
    }

    // GIM Data Formats
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
    }

    public abstract class GimPaletteCodec
    {
        public GimPaletteDecoder Decode;
        public string Encode;
        public GimPixelFormat Format;
    }

    public abstract class GimDataCodec
    {
        public GimDataDecoder Decode;
        public string Encode;
        public GimDataFormat Format;
    }

    // GIM Palette Format Classes
    public class GimPaletteCodec_Rgb565 : GimPaletteCodec
    {
        public GimPaletteCodec_Rgb565()
        {
            Decode = new GimPaletteDecoder_Rgb565();
            Encode = null;
            Format = GimPixelFormat.Rgb565;
        }
    }
    public class GimPaletteCodec_Argb1555 : GimPaletteCodec
    {
        public GimPaletteCodec_Argb1555()
        {
            Decode = new GimPaletteDecoder_Argb1555();
            Encode = null;
            Format = GimPixelFormat.Argb1555;
        }
    }
    public class GimPaletteCodec_Argb4444 : GimPaletteCodec
    {
        public GimPaletteCodec_Argb4444()
        {
            Decode = new GimPaletteDecoder_Argb4444();
            Encode = null;
            Format = GimPixelFormat.Argb4444;
        }
    }
    public class GimPaletteCodec_Argb8888 : GimPaletteCodec
    {
        public GimPaletteCodec_Argb8888()
        {
            Decode = new GimPaletteDecoder_Argb8888();
            Encode = null;
            Format = GimPixelFormat.Argb8888;
        }
    }

    // GIM Data Format Classes
    public class GimDataCodec_Rgb565 : GimDataCodec
    {
        public GimDataCodec_Rgb565()
        {
            Decode = new GimDataDecoder_Rgb565();
            Encode = null;
            Format = GimDataFormat.Rgb565;
        }
    }
    public class GimDataCodec_Argb1555 : GimDataCodec
    {
        public GimDataCodec_Argb1555()
        {
            Decode = new GimDataDecoder_Argb1555();
            Encode = null;
            Format = GimDataFormat.Argb1555;
        }
    }
    public class GimDataCodec_Argb4444 : GimDataCodec
    {
        public GimDataCodec_Argb4444()
        {
            Decode = new GimDataDecoder_Argb4444();
            Encode = null;
            Format = GimDataFormat.Argb4444;
        }
    }
    public class GimDataCodec_Argb8888 : GimDataCodec
    {
        public GimDataCodec_Argb8888()
        {
            Decode = new GimDataDecoder_Argb8888();
            Encode = null;
            Format = GimDataFormat.Argb8888;
        }
    }
    public class GimDataCodec_Index4 : GimDataCodec
    {
        public GimDataCodec_Index4()
        {
            Decode = new GimDataDecoder_Index4();
            Encode = null;
            Format = GimDataFormat.Index4;
        }
    }
    public class GimDataCodec_Index8 : GimDataCodec
    {
        public GimDataCodec_Index8()
        {
            Decode = new GimDataDecoder_Index8();
            Encode = null;
            Format = GimDataFormat.Index8;
        }
    }
    public class GimDataCodec_Index16 : GimDataCodec
    {
        public GimDataCodec_Index16()
        {
            Decode = new GimDataDecoder_Index16();
            Encode = null;
            Format = GimDataFormat.Index16;
        }
    }
    public class GimDataCodec_Index32 : GimDataCodec
    {
        public GimDataCodec_Index32()
        {
            Decode = new GimDataDecoder_Index32();
            Encode = null;
            Format = GimDataFormat.Index32;
        }
    }

    // GimCodecs is a static class for containing codecs
    // Register your codecs here in the initialize function.
    public static class GimCodecs
    {
        private static bool init = false;

        public static Dictionary<byte, GimPaletteCodec> GimPaletteCodecs = new Dictionary<byte, GimPaletteCodec>();
        public static Dictionary<byte, GimDataCodec> GimDataCodecs       = new Dictionary<byte, GimDataCodec>();

        // Initalize
        public static void Initalize()
        {
            // Add the Palette Formats
            GimPaletteCodecs.Add(0x00, new GimPaletteCodec_Rgb565());
            GimPaletteCodecs.Add(0x01, new GimPaletteCodec_Argb1555());
            GimPaletteCodecs.Add(0x02, new GimPaletteCodec_Argb4444());
            GimPaletteCodecs.Add(0x03, new GimPaletteCodec_Argb8888());

            // Add the Data Formats
            GimDataCodecs.Add(0x00, new GimDataCodec_Rgb565());
            GimDataCodecs.Add(0x01, new GimDataCodec_Argb1555());
            GimDataCodecs.Add(0x02, new GimDataCodec_Argb4444());
            GimDataCodecs.Add(0x03, new GimDataCodec_Argb8888());
            GimDataCodecs.Add(0x04, new GimDataCodec_Index4());
            GimDataCodecs.Add(0x05, new GimDataCodec_Index8());
            GimDataCodecs.Add(0x06, new GimDataCodec_Index16());
            GimDataCodecs.Add(0x07, new GimDataCodec_Index32());

            init = true;
        }

        public static GimPaletteCodec GetPaletteCodec(byte Codec)
        {
            if (!init) Initalize();

            if (GimPaletteCodecs.ContainsKey(Codec))
                return GimPaletteCodecs[Codec];

            return null;
        }
        public static GimDataCodec GetDataCodec(byte Codec)
        {
            if (!init) Initalize();

            if (GimDataCodecs.ContainsKey(Codec))
                return GimDataCodecs[Codec];

            return null;
        }
    }
}