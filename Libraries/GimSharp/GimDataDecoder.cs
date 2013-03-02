using System;

namespace GimSharp
{
    public abstract class GimDataDecoder
    {
        // Decode Palette
        public abstract bool DecodePalette(ref byte[] Input, int Pointer);

        // Initalize
        public abstract bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries);

        // Decode Data
        public abstract bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output);

        // Unswizzle Data
        public abstract void UnSwizzle(ref byte[] Input, int Pointer);
    }

    public class GimDataDecoder_Rgb565 : GimDataDecoder
    {
        int width, height;
        bool init = false;

        public override bool DecodePalette(ref byte[] Input, int Pointer)
        {
            return true;
        }
        public override bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output)
        {
            if (!init) throw new Exception("Could not decode data because you have not initalized yet.");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get entry
                    ushort entry = BitConverter.ToUInt16(Input, Pointer);

                    Output[((y * width) + x) * 4 + 0] = 0xFF;
                    Output[((y * width) + x) * 4 + 1] = (byte)(((entry >> 0)  & 0x1F) * 0xFF / 0x1F);
                    Output[((y * width) + x) * 4 + 2] = (byte)(((entry >> 5)  & 0x3F) * 0xFF / 0x3F);
                    Output[((y * width) + x) * 4 + 3] = (byte)(((entry >> 11) & 0x1F) * 0xFF / 0x1F);
                    Pointer += 2;
                }
            }

            return true;
        }

        // Initalize
        public override bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries)
        {
            width  = Width;
            height = Height;
            init   = true;

            return true;
        }

        // Unswizzle
        public override void UnSwizzle(ref byte[] Input, int Pointer)
        {
            GimSwizzle.UnSwizzle(ref Input, Pointer, width * 2, height);
        }
    }

    public class GimDataDecoder_Argb1555 : GimDataDecoder
    {
        int width, height;
        bool init = false;

        public override bool DecodePalette(ref byte[] Input, int Pointer)
        {
            return true;
        }
        public override bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output)
        {
            if (!init) throw new Exception("Could not decode data because you have not initalized yet.");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get entry
                    ushort entry = BitConverter.ToUInt16(Input, Pointer);

                    Output[((y * width) + x) * 4 + 0] = (byte)(((entry >> 15) & 0x01) * 0xFF);
                    Output[((y * width) + x) * 4 + 1] = (byte)(((entry >> 0)  & 0x1F) * 0xFF / 0x1F);
                    Output[((y * width) + x) * 4 + 2] = (byte)(((entry >> 5)  & 0x1F) * 0xFF / 0x1F);
                    Output[((y * width) + x) * 4 + 3] = (byte)(((entry >> 10) & 0x1F) * 0xFF / 0x1F);
                    Pointer += 2;
                }
            }

            return true;
        }

        // Initalize
        public override bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries)
        {
            width  = Width;
            height = Height;
            init   = true;

            return true;
        }

        // Unswizzle
        public override void UnSwizzle(ref byte[] Input, int Pointer)
        {
            GimSwizzle.UnSwizzle(ref Input, Pointer, width * 2, height);
        }

    }

    public class GimDataDecoder_Argb4444 : GimDataDecoder
    {
        int width, height;
        bool init = false;

        public override bool DecodePalette(ref byte[] Input, int Pointer)
        {
            return true;
        }
        public override bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output)
        {
            if (!init) throw new Exception("Could not decode data because you have not initalized yet.");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get entry
                    ushort entry = BitConverter.ToUInt16(Input, Pointer);

                    Output[((y * width) + x) * 4 + 0] = (byte)(((entry >> 12) & 0xF) * 0xFF / 0xF);
                    Output[((y * width) + x) * 4 + 1] = (byte)(((entry >> 0)  & 0xF) * 0xFF / 0xF);
                    Output[((y * width) + x) * 4 + 2] = (byte)(((entry >> 4)  & 0xF) * 0xFF / 0xF);
                    Output[((y * width) + x) * 4 + 3] = (byte)(((entry >> 8)  & 0xF) * 0xFF / 0xF);
                    Pointer += 2;
                }
            }

            return true;
        }

        // Initalize
        public override bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries)
        {
            width  = Width;
            height = Height;
            init   = true;

            return true;
        }

        // Unswizzle
        public override void UnSwizzle(ref byte[] Input, int Pointer)
        {
            GimSwizzle.UnSwizzle(ref Input, Pointer, width * 2, height);
        }
    }

    public class GimDataDecoder_Argb8888 : GimDataDecoder
    {
        int width, height;
        bool init = false;

        public override bool DecodePalette(ref byte[] Input, int Pointer)
        {
            return true;
        }
        public override bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output)
        {
            if (!init) throw new Exception("Could not decode data because you have not initalized yet.");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get entry
                    uint entry = BitConverter.ToUInt32(Input, Pointer);

                    Output[((y * width) + x) * 4 + 0] = (byte)((entry >> 24) & 0xFF);
                    Output[((y * width) + x) * 4 + 1] = (byte)((entry >> 0)  & 0xFF);
                    Output[((y * width) + x) * 4 + 2] = (byte)((entry >> 8)  & 0xFF);
                    Output[((y * width) + x) * 4 + 3] = (byte)((entry >> 16) & 0xFF);
                    Pointer += 4;
                }
            }

            return true;
        }

        // Initalize
        public override bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries)
        {
            width  = Width;
            height = Height;
            init   = true;

            return true;
        }

        // Unswizzle
        public override void UnSwizzle(ref byte[] Input, int Pointer)
        {
            GimSwizzle.UnSwizzle(ref Input, Pointer, width * 4, height);
        }
    }

    public class GimDataDecoder_Index4 : GimDataDecoder
    {
        int width, height;
        bool init = false;
        byte[][] Palette;
        GimPaletteDecoder paletteDecoder;

        public override bool DecodePalette(ref byte[] Input, int Pointer)
        {
            paletteDecoder.DecodePalette(ref Input, Pointer, ref Palette);
            return true;
        }
        public override bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output)
        {
            if (!init) throw new Exception("Could not decode data because you have not initalized yet.");
            if (Palette.Length == 0) throw new Exception("Could not decode data because there are no entries in the palette.");

            int PixelPointer = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get entry
                    byte entry = (byte)((Input[Pointer + (PixelPointer >> 1)] >> ((PixelPointer % 2) * 4)) & 0xF);

                    Output[((y * width) + x) * 4 + 0] = Palette[entry][0];
                    Output[((y * width) + x) * 4 + 1] = Palette[entry][1];
                    Output[((y * width) + x) * 4 + 2] = Palette[entry][2];
                    Output[((y * width) + x) * 4 + 3] = Palette[entry][3];
                    PixelPointer++;
                }
            }

            return true;
        }

        // Initalize
        public override bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries)
        {
            width  = Width;
            height = Height;

            Palette = new byte[PaletteEntries][];
            paletteDecoder = PaletteDecoder;

            init = true;

            return true;
        }

        // Unswizzle
        public override void UnSwizzle(ref byte[] Input, int Pointer)
        {
            GimSwizzle.UnSwizzle(ref Input, Pointer, width / 2, height);
        }
    }

    public class GimDataDecoder_Index8 : GimDataDecoder
    {
        int width, height;
        bool init = false;
        byte[][] Palette;
        GimPaletteDecoder paletteDecoder;

        public override bool DecodePalette(ref byte[] Input, int Pointer)
        {
            paletteDecoder.DecodePalette(ref Input, Pointer, ref Palette);
            return true;
        }
        public override bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output)
        {
            if (!init) throw new Exception("Could not decode data because you have not initalized yet.");
            if (Palette.Length == 0) throw new Exception("Could not decode data because there are no entries in the palette.");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get entry
                    byte entry = Input[Pointer];

                    Output[((y * width) + x) * 4 + 0] = Palette[entry][0];
                    Output[((y * width) + x) * 4 + 1] = Palette[entry][1];
                    Output[((y * width) + x) * 4 + 2] = Palette[entry][2];
                    Output[((y * width) + x) * 4 + 3] = Palette[entry][3];
                    Pointer++;
                }
            }

            return true;
        }

        // Initalize
        public override bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries)
        {
            width  = Width;
            height = Height;

            Palette = new byte[PaletteEntries][];
            paletteDecoder = PaletteDecoder;

            init = true;

            return true;
        }

        // Unswizzle
        public override void UnSwizzle(ref byte[] Input, int Pointer)
        {
            GimSwizzle.UnSwizzle(ref Input, Pointer, width, height);
        }
    }

    public class GimDataDecoder_Index16 : GimDataDecoder
    {
        int width, height;
        bool init = false;
        byte[][] Palette;
        GimPaletteDecoder paletteDecoder;

        public override bool DecodePalette(ref byte[] Input, int Pointer)
        {
            paletteDecoder.DecodePalette(ref Input, Pointer, ref Palette);
            return true;
        }
        public override bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output)
        {
            if (!init) throw new Exception("Could not decode data because you have not initalized yet.");
            if (Palette.Length == 0) throw new Exception("Could not decode data because there are no entries in the palette.");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get entry
                    ushort entry = BitConverter.ToUInt16(Input, Pointer);

                    Output[((y * width) + x) * 4 + 0] = Palette[entry][0];
                    Output[((y * width) + x) * 4 + 1] = Palette[entry][1];
                    Output[((y * width) + x) * 4 + 2] = Palette[entry][2];
                    Output[((y * width) + x) * 4 + 3] = Palette[entry][3];
                    Pointer += 2;
                }
            }

            return true;
        }

        // Initalize
        public override bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries)
        {
            width  = Width;
            height = Height;

            Palette = new byte[PaletteEntries][];
            paletteDecoder = PaletteDecoder;

            init = true;

            return true;
        }

        // Unswizzle
        public override void UnSwizzle(ref byte[] Input, int Pointer)
        {
            GimSwizzle.UnSwizzle(ref Input, Pointer, width * 2, height);
        }
    }

    public class GimDataDecoder_Index32 : GimDataDecoder
    {
        int width, height;
        bool init = false;
        byte[][] Palette;
        GimPaletteDecoder paletteDecoder;

        public override bool DecodePalette(ref byte[] Input, int Pointer)
        {
            paletteDecoder.DecodePalette(ref Input, Pointer, ref Palette);
            return true;
        }
        public override bool DecodeData(ref byte[] Input, int Pointer, ref byte[] Output)
        {
            if (!init) throw new Exception("Could not decode data because you have not initalized yet.");
            if (Palette.Length == 0) throw new Exception("Could not decode data because there are no entries in the palette.");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get entry
                    uint entry = BitConverter.ToUInt32(Input, Pointer);

                    Output[((y * width) + x) * 4 + 0] = Palette[entry][0];
                    Output[((y * width) + x) * 4 + 1] = Palette[entry][1];
                    Output[((y * width) + x) * 4 + 2] = Palette[entry][2];
                    Output[((y * width) + x) * 4 + 3] = Palette[entry][3];
                    Pointer += 4;
                }
            }

            return true;
        }

        // Initalize
        public override bool Initalize(int Width, int Height, GimPaletteDecoder PaletteDecoder, int PaletteEntries)
        {
            width  = Width;
            height = Height;

            Palette = new byte[PaletteEntries][];
            paletteDecoder = PaletteDecoder;

            init = true;

            return true;
        }

        // Unswizzle
        public override void UnSwizzle(ref byte[] Input, int Pointer)
        {
            GimSwizzle.UnSwizzle(ref Input, Pointer, width * 4, height);
        }
    }
}