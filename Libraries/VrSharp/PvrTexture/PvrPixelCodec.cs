using System;

namespace VrSharp.PvrTexture
{
    public abstract class PvrPixelCodec : VrPixelCodec
    {
        #region Argb1555
        // Argb1555
        public class Argb1555 : PvrPixelCodec
        {
            public override bool CanDecode() { return true; }
            public override bool CanEncode() { return true; }
            public override int GetBpp() { return 16; }

            public override byte[,] GetClut(byte[] input, int offset, int entries)
            {
                byte[,] clut = new byte[entries, 4];

                for (int i = 0; i < entries; i++)
                {
                    ushort pixel = BitConverter.ToUInt16(input, offset);

                    clut[i, 3] = (byte)(((pixel >> 15) & 0x01) * 0xFF);
                    clut[i, 2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
                    clut[i, 1] = (byte)(((pixel >> 5)  & 0x1F) * 0xFF / 0x1F);
                    clut[i, 0] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);

                    offset += 2;
                }

                return clut;
            }

            public override byte[] GetPixelPalette(byte[] input, int offset)
            {
                ushort pixel   = BitConverter.ToUInt16(input, offset);
                byte[] palette = new byte[4];

                palette[3] = (byte)(((pixel >> 15) & 0x01) * 0xFF);
                palette[2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
                palette[1] = (byte)(((pixel >> 5)  & 0x1F) * 0xFF / 0x1F);
                palette[0] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);

                return palette;
            }

            public override byte[] CreateClut(byte[,] input)
            {
                int offset  = 0;
                byte[] clut = new byte[input.GetLength(0) * (GetBpp() / 8)];

                for (int i = 0; i < input.GetLength(0); i++)
                {
                    ushort pixel = 0x0000;
                    pixel |= (ushort)(((input[i, 3]        / 0xFF) & 0x01) << 15);
                    pixel |= (ushort)(((input[i, 2] * 0x1F / 0xFF) & 0x1F) << 10);
                    pixel |= (ushort)(((input[i, 1] * 0x1F / 0xFF) & 0x1F) << 5);
                    pixel |= (ushort)(((input[i, 0] * 0x1F / 0xFF) & 0x1F) << 0);

                    BitConverter.GetBytes(pixel).CopyTo(clut, offset);
                    offset += (GetBpp() / 8);
                }

                return clut;
            }

            public override byte[] CreatePixelPalette(byte[] input, int offset)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)(((input[offset + 3]        / 0xFF) & 0x01) << 15);
                pixel |= (ushort)(((input[offset + 2] * 0x1F / 0xFF) & 0x1F) << 10);
                pixel |= (ushort)(((input[offset + 1] * 0x1F / 0xFF) & 0x1F) << 5);
                pixel |= (ushort)(((input[offset + 0] * 0x1F / 0xFF) & 0x1F) << 0);

                return BitConverter.GetBytes(pixel);
            }
        }
        #endregion

        #region Rgb565
        // Rgb565
        public class Rgb565 : PvrPixelCodec
        {
            public override bool CanDecode() { return true; }
            public override bool CanEncode() { return true; }
            public override int GetBpp() { return 16; }

            public override byte[,] GetClut(byte[] input, int offset, int entries)
            {
                byte[,] clut = new byte[entries, 4];

                for (int i = 0; i < entries; i++)
                {
                    ushort pixel = BitConverter.ToUInt16(input, offset);

                    clut[i, 3] = 0xFF;
                    clut[i, 2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
                    clut[i, 1] = (byte)(((pixel >> 5)  & 0x3F) * 0xFF / 0x3F);
                    clut[i, 0] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);

                    offset += 2;
                }

                return clut;
            }

            public override byte[] GetPixelPalette(byte[] input, int offset)
            {
                ushort pixel   = BitConverter.ToUInt16(input, offset);
                byte[] palette = new byte[4];

                palette[3] = 0xFF;
                palette[2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
                palette[1] = (byte)(((pixel >> 5)  & 0x3F) * 0xFF / 0x3F);
                palette[0] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);

                return palette;
            }

            public override byte[] CreateClut(byte[,] input)
            {
                int offset  = 0;
                byte[] clut = new byte[input.GetLength(0) * (GetBpp() / 8)];

                for (int i = 0; i < input.GetLength(0); i++)
                {
                    ushort pixel = 0x0000;
                    pixel |= (ushort)(((input[i, 2] * 0x1F / 0xFF) & 0x1F) << 11);
                    pixel |= (ushort)(((input[i, 1] * 0x3F / 0xFF) & 0x3F) << 5);
                    pixel |= (ushort)(((input[i, 0] * 0x1F / 0xFF) & 0x1F) << 0);

                    BitConverter.GetBytes(pixel).CopyTo(clut, offset);
                    offset += (GetBpp() / 8);
                }

                return clut;
            }

            public override byte[] CreatePixelPalette(byte[] input, int offset)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)(((input[offset + 2] * 0x1F / 0xFF) & 0x1F) << 11);
                pixel |= (ushort)(((input[offset + 1] * 0x3F / 0xFF) & 0x3F) << 5);
                pixel |= (ushort)(((input[offset + 0] * 0x1F / 0xFF) & 0x1F) << 0);

                return BitConverter.GetBytes(pixel);
            }
        }
        #endregion

        #region Argb4444
        // Argb4444
        public class Argb4444 : PvrPixelCodec
        {
            public override bool CanDecode() { return true; }
            public override bool CanEncode() { return true; }
            public override int GetBpp() { return 16; }

            public override byte[,] GetClut(byte[] input, int offset, int entries)
            {
                byte[,] clut = new byte[entries, 4];

                for (int i = 0; i < entries; i++)
                {
                    ushort pixel = BitConverter.ToUInt16(input, offset);

                    clut[i, 3] = (byte)(((pixel >> 12) & 0x0F) * 0xFF / 0x0F);
                    clut[i, 2] = (byte)(((pixel >> 8)  & 0x0F) * 0xFF / 0x0F);
                    clut[i, 1] = (byte)(((pixel >> 4)  & 0x0F) * 0xFF / 0x0F);
                    clut[i, 0] = (byte)(((pixel >> 0)  & 0x0F) * 0xFF / 0x0F);

                    offset += 2;
                }

                return clut;
            }

            public override byte[] GetPixelPalette(byte[] input, int offset)
            {
                ushort pixel   = BitConverter.ToUInt16(input, offset);
                byte[] palette = new byte[4];

                palette[3] = (byte)(((pixel >> 12) & 0x0F) * 0xFF / 0x0F);
                palette[2] = (byte)(((pixel >> 8)  & 0x0F) * 0xFF / 0x0F);
                palette[1] = (byte)(((pixel >> 4)  & 0x0F) * 0xFF / 0x0F);
                palette[0] = (byte)(((pixel >> 0)  & 0x0F) * 0xFF / 0x0F);

                return palette;
            }

            public override byte[] CreateClut(byte[,] input)
            {
                int offset  = 0;
                byte[] clut = new byte[input.GetLength(0) * (GetBpp() / 8)];

                for (int i = 0; i < input.GetLength(0); i++)
                {
                    ushort pixel = 0x0000;
                    pixel |= (ushort)(((input[i, 3] * 0x0F / 0xFF) & 0x0F) << 12);
                    pixel |= (ushort)(((input[i, 2] * 0x0F / 0xFF) & 0x0F) << 8);
                    pixel |= (ushort)(((input[i, 1] * 0x0F / 0xFF) & 0x0F) << 4);
                    pixel |= (ushort)(((input[i, 0] * 0x0F / 0xFF) & 0x0F) << 0);

                    BitConverter.GetBytes(pixel).CopyTo(clut, offset);
                    offset += (GetBpp() / 8);
                }

                return clut;
            }

            public override byte[] CreatePixelPalette(byte[] input, int offset)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)(((input[offset + 3] * 0x0F / 0xFF) & 0x0F) << 12);
                pixel |= (ushort)(((input[offset + 2] * 0x0F / 0xFF) & 0x0F) << 8);
                pixel |= (ushort)(((input[offset + 1] * 0x0F / 0xFF) & 0x0F) << 4);
                pixel |= (ushort)(((input[offset + 0] * 0x0F / 0xFF) & 0x0F) << 0);

                return BitConverter.GetBytes(pixel);
            }
        }
        #endregion
    }
}