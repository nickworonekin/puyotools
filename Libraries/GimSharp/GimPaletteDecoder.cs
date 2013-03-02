using System;

namespace GimSharp
{
    public abstract class GimPaletteDecoder
    {
        // Get bits per pixel
        public abstract int GetBpp();

        // Decode Palette
        public abstract bool DecodePalette(ref byte[] Buf, int Pointer, ref byte[][] Palette);
    }

    public class GimPaletteDecoder_Rgb565 : GimPaletteDecoder
    {
        public override int GetBpp()
        {
            return 16;
        }

        public override bool DecodePalette(ref byte[] Buf, int Pointer, ref byte[][] Palette)
        {
            for (int i = 0; i < Palette.Length; i++)
            {
                Palette[i] = new byte[4];

                // Get entry
                ushort entry = BitConverter.ToUInt16(Buf, Pointer);

                Palette[i][0] = 0xFF;
                Palette[i][1] = (byte)(((entry >> 0)  & 0x1F) * 0xFF / 0x1F);
                Palette[i][2] = (byte)(((entry >> 5)  & 0x3F) * 0xFF / 0x3F);
                Palette[i][3] = (byte)(((entry >> 11) & 0x1F) * 0xFF / 0x1F);
                Pointer += 2;
            }

            return true;
        }
    }

    public class GimPaletteDecoder_Argb1555 : GimPaletteDecoder
    {
        public override int GetBpp()
        {
            return 16;
        }

        public override bool DecodePalette(ref byte[] Buf, int Pointer, ref byte[][] Palette)
        {
            for (int i = 0; i < Palette.Length; i++)
            {
                Palette[i] = new byte[4];

                // Get entry
                ushort entry = BitConverter.ToUInt16(Buf, Pointer);

                Palette[i][0] = (byte)(((entry >> 15) & 0x01) * 0xFF);
                Palette[i][1] = (byte)(((entry >> 0)  & 0x1F) * 0xFF / 0x1F);
                Palette[i][2] = (byte)(((entry >> 5)  & 0x1F) * 0xFF / 0x1F);
                Palette[i][3] = (byte)(((entry >> 10) & 0x1F) * 0xFF / 0x1F);
                Pointer += 2;
            }

            return true;
        }
    }

    public class GimPaletteDecoder_Argb4444 : GimPaletteDecoder
    {
        public override int GetBpp()
        {
            return 16;
        }

        public override bool DecodePalette(ref byte[] Buf, int Pointer, ref byte[][] Palette)
        {
            for (int i = 0; i < Palette.Length; i++)
            {
                Palette[i] = new byte[4];

                // Get entry
                ushort entry = BitConverter.ToUInt16(Buf, Pointer);

                Palette[i][0] = (byte)(((entry >> 12) & 0xF) * 0xFF);
                Palette[i][1] = (byte)(((entry >> 0)  & 0xF) * 0xFF / 0xF);
                Palette[i][2] = (byte)(((entry >> 4)  & 0xF) * 0xFF / 0xF);
                Palette[i][3] = (byte)(((entry >> 8)  & 0xF) * 0xFF / 0xF);
                Pointer += 2;
            }

            return true;
        }
    }

    public class GimPaletteDecoder_Argb8888 : GimPaletteDecoder
    {
        public override int GetBpp()
        {
            return 32;
        }

        public override bool DecodePalette(ref byte[] Buf, int Pointer, ref byte[][] Palette)
        {
            for (int i = 0; i < Palette.Length; i++)
            {
                Palette[i] = new byte[4];

                // Get entry
                uint entry = BitConverter.ToUInt32(Buf, Pointer);

                Palette[i][0] = (byte)((entry >> 24) & 0xFF);
                Palette[i][1] = (byte)((entry >> 0)  & 0xFF);
                Palette[i][2] = (byte)((entry >> 8)  & 0xFF);
                Palette[i][3] = (byte)((entry >> 16) & 0xFF);
                Pointer += 4;
            }

            return true;
        }
    }
}