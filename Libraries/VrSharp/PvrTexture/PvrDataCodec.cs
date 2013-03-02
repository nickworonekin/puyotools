using System;

namespace VrSharp.PvrTexture
{
    public abstract class PvrDataCodec : VrDataCodec
    {
        #region Square Twiddled
        // Square Twiddled
        public class SquareTwiddled : PvrDataCodec
        {
            public override bool CanDecode() { return true; }
            public override bool CanEncode() { return true; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return PixelCodec.GetBpp(); }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                int StartOffset = offset;
                byte[] output   = new byte[width * height * 4];
                TwiddledMap TwiddledMap = new TwiddledMap(width, GetBpp(PixelCodec));

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte[] palette = PixelCodec.GetPixelPalette(input, StartOffset + TwiddledMap.GetTwiddledOffset(x, y));

                        output[(((y * width) + x) * 4) + 3] = palette[3];
                        output[(((y * width) + x) * 4) + 2] = palette[2];
                        output[(((y * width) + x) * 4) + 1] = palette[1];
                        output[(((y * width) + x) * 4) + 0] = palette[0];

                        offset += (GetBpp(PixelCodec) / 8);
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * (GetBpp(PixelCodec) / 8)];
                TwiddledMap TwiddledMap = new TwiddledMap(width, GetBpp(PixelCodec));

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte[] palette = PixelCodec.CreatePixelPalette(input, (((y * width) + x) * 4));
                        palette.CopyTo(output, TwiddledMap.GetTwiddledOffset(x, y));
                    }
                }

                return output;
            }
        }
        #endregion

        #region Square Twiddled with Mipmaps
        // Square Twiddled with Mipmaps
        public class SquareTwiddledMipmaps : PvrDataCodec
        {
            public override bool CanDecode() { return true;  }
            public override bool CanEncode() { return false; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return PixelCodec.GetBpp(); }
            public override bool ContainsMipmaps() { return true; }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                int StartOffset = offset;
                byte[] output   = new byte[width * height * 4];
                TwiddledMap TwiddledMap = new TwiddledMap(width, GetBpp(PixelCodec));

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte[] palette = PixelCodec.GetPixelPalette(input, StartOffset + TwiddledMap.GetTwiddledOffset(x, y));

                        output[(((y * width) + x) * 4) + 3] = palette[3];
                        output[(((y * width) + x) * 4) + 2] = palette[2];
                        output[(((y * width) + x) * 4) + 1] = palette[1];
                        output[(((y * width) + x) * 4) + 0] = palette[0];

                        offset += (GetBpp(PixelCodec) / 8);
                    }
                }

                return output;
            }

            public override byte[] DecodeMipmap(byte[] input, int offset, int mipmap, int width, int height, VrPixelCodec PixelCodec)
            {
                // Get the width of the mipmap and go to the correct offset
                int MipmapWidth = width;
                for (int i = 0; i < mipmap; i++)
                    MipmapWidth >>= 1;

                for (int i = 1; i < MipmapWidth; i <<= 1)
                    offset += Math.Max(i * i * (GetBpp(PixelCodec) / 8), 4);

                return Decode(input, offset, MipmapWidth, MipmapWidth, PixelCodec);
            }

            public override byte[] Encode(byte[] data, int width, int height, VrPixelCodec PixelCodec)
            {
                return null;
            }
        }
        #endregion

        #region Vq
        // Vq
        public class Vq : PvrDataCodec
        {
            public override bool CanDecode() { return true;  }
            public override bool CanEncode() { return false; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return 8; }
            public override int GetNumClutEntries() { return 1024; }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                int StartOffset = offset;
                byte[] output   = new byte[width * height * 4];
                byte[,] clut    = ClutData;

                TwiddledMap TwiddledMap = new TwiddledMap(width / 2, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += 2)
                {
                    for (int x = 0; x < width; x += 2)
                    {
                        ushort entry = (ushort)(input[StartOffset + TwiddledMap.GetTwiddledOffset(x >> 1, y >> 1)] * 4);

                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            for (int x2 = 0; x2 < 2; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = clut[entry + (x2 * 2) + y2, 3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = clut[entry + (x2 * 2) + y2, 2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = clut[entry + (x2 * 2) + y2, 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = clut[entry + (x2 * 2) + y2, 0];
                            }
                        }

                        offset++;
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] data, int width, int height, VrPixelCodec PixelCodec)
            {
                return null;
            }
        }
        #endregion

        #region Vq with Mipmaps
        // Vq with Mipmaps
        public class VqMipmaps : PvrDataCodec
        {
            public override bool CanDecode() { return true;  }
            public override bool CanEncode() { return false; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return 8; }
            public override int GetNumClutEntries() { return 1024; }
            public override bool ContainsMipmaps() { return true; }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                int StartOffset = offset;
                byte[] output   = new byte[width * height * 4];
                byte[,] clut    = ClutData;

                TwiddledMap TwiddledMap = new TwiddledMap(width / 2, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += 2)
                {
                    for (int x = 0; x < width; x += 2)
                    {
                        ushort entry = (ushort)(input[StartOffset + TwiddledMap.GetTwiddledOffset(x >> 1, y >> 1)] * 4);

                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            for (int x2 = 0; x2 < 2; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = clut[entry + (x2 * 2) + y2, 3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = clut[entry + (x2 * 2) + y2, 2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = clut[entry + (x2 * 2) + y2, 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = clut[entry + (x2 * 2) + y2, 0];
                            }
                        }

                        offset++;
                    }
                }

                return output;
            }

            public override byte[] DecodeMipmap(byte[] input, int offset, int mipmap, int width, int height, VrPixelCodec PixelCodec)
            {
                // Get the width of the mipmap and go to the correct offset
                int MipmapWidth = width;
                for (int i = 0; i < mipmap; i++)
                    MipmapWidth >>= 1;

                for (int i = 1; i < MipmapWidth; i <<= 1)
                    offset += (Math.Max(i * i, 4) >> 2);

                return Decode(input, offset, MipmapWidth, MipmapWidth, PixelCodec);
            }

            public override byte[] Encode(byte[] data, int width, int height, VrPixelCodec PixelCodec)
            {
                return null;
            }
        }
        #endregion

        #region 4-bit Texture with External Clut
        // 4-bit Texture with External Clut
        public class Index4 : PvrDataCodec
        {
            public override bool CanDecode() { return true; }
            public override bool CanEncode() { return true; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return 4; }
            public override int GetNumClutEntries()  { return 16; }
            public override bool NeedsExternalClut() { return true; }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                byte[,] clut  = ClutData;
                int ChunkSize = Math.Min(width, height);
                TwiddledMap TwiddledMap = new TwiddledMap(ChunkSize, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += ChunkSize)
                {
                    for (int x = 0; x < width; x += ChunkSize)
                    {
                        int StartOffset = offset;

                        for (int y2 = 0; y2 < ChunkSize; y2++)
                        {
                            for (int x2 = 0; x2 < ChunkSize; x2++)
                            {
                                byte entry = input[StartOffset + TwiddledMap.GetTwiddledOffset(x2, y2)];
                                entry = (byte)((entry >> ((y2 & 0x01) * 4)) & 0x0F);

                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = clut[entry, 3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = clut[entry, 2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = clut[entry, 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = clut[entry, 0];

                                if ((x2 & 0x01) != 0)
                                    offset++;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                return null;
            }
        }
        #endregion

        #region 8-bit Texture with External Clut
        // 8-bit Texture with External Clut
        public class Index8 : PvrDataCodec
        {
            public override bool CanDecode() { return true; }
            public override bool CanEncode() { return true; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return 8; }
            public override int GetNumClutEntries()  { return 256; }
            public override bool NeedsExternalClut() { return true; }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                byte[,] clut  = ClutData;
                int ChunkSize = Math.Min(width, height);
                TwiddledMap TwiddledMap = new TwiddledMap(ChunkSize, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += ChunkSize)
                {
                    for (int x = 0; x < width; x += ChunkSize)
                    {
                        int StartOffset = offset;

                        for (int y2 = 0; y2 < ChunkSize; y2++)
                        {
                            for (int x2 = 0; x2 < ChunkSize; x2++)
                            {
                                byte entry = input[StartOffset + TwiddledMap.GetTwiddledOffset(x2, y2)];

                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = clut[entry, 3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = clut[entry, 2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = clut[entry, 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = clut[entry, 0];

                                offset++;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset = 0;
                byte[] output = new byte[width * height];
                int ChunkSize = Math.Min(width, height);
                TwiddledMap TwiddledMap = new TwiddledMap(ChunkSize, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += ChunkSize)
                {
                    for (int x = 0; x < width; x += ChunkSize)
                    {
                        int StartOffset = offset;

                        for (int y2 = 0; y2 < ChunkSize; y2++)
                        {
                            for (int x2 = 0; x2 < ChunkSize; x2++)
                            {
                                output[StartOffset + TwiddledMap.GetTwiddledOffset(x2, y2)] = input[(((y + y2) * width) + (x + x2))];
                                offset++;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Rectangle
        // Rectangle
        public class Rectangle : PvrDataCodec
        {
            public override bool CanDecode() { return true; }
            public override bool CanEncode() { return true; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return PixelCodec.GetBpp(); }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y ++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte[] palette = PixelCodec.GetPixelPalette(input, offset);

                        output[(((y * width) + x) * 4) + 3] = palette[3];
                        output[(((y * width) + x) * 4) + 2] = palette[2];
                        output[(((y * width) + x) * 4) + 1] = palette[1];
                        output[(((y * width) + x) * 4) + 0] = palette[0];

                        offset += (GetBpp(PixelCodec) / 8);
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset    = 0;
                byte[] output = new byte[width * height * (GetBpp(PixelCodec) / 8)];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte[] palette = PixelCodec.CreatePixelPalette(input, (((y * width) + x) * 4));
                        palette.CopyTo(output, offset);

                        offset += (GetBpp(PixelCodec) / 8);
                    }
                }

                return output;
            }
        }
        #endregion

        #region Rectangle Twiddled
        // Rectangle Twiddled
        public class RectangleTwiddled : PvrDataCodec
        {
            public override bool CanDecode() { return true; }
            public override bool CanEncode() { return true; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return PixelCodec.GetBpp(); }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int ChunkSize = Math.Min(width, height);

                TwiddledMap TwiddledMap = new TwiddledMap(ChunkSize, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += ChunkSize)
                {
                    for (int x = 0; x < width; x += ChunkSize)
                    {
                        int StartOffset = offset;
                        
                        for (int y2 = 0; y2 < ChunkSize; y2++)
                        {
                            for (int x2 = 0; x2 < ChunkSize; x2++)
                            {
                                byte[] palette = PixelCodec.GetPixelPalette(input, StartOffset + TwiddledMap.GetTwiddledOffset(x2, y2));

                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = palette[3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = palette[2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = palette[1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = palette[0];

                                offset += (GetBpp(PixelCodec) / 8);
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset    = 0;
                byte[] output = new byte[width * height * (GetBpp(PixelCodec) / 8)];
                int ChunkSize = Math.Min(width, height);

                TwiddledMap TwiddledMap = new TwiddledMap(width, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += ChunkSize)
                {
                    for (int x = 0; x < width; x += ChunkSize)
                    {
                        int StartOffset = offset;

                        for (int y2 = 0; y2 < ChunkSize; y2++)
                        {
                            for (int x2 = 0; x2 < ChunkSize; x2++)
                            {
                                byte[] palette = PixelCodec.CreatePixelPalette(input, ((((y + y2) * width) + (x + x2)) * 4));
                                palette.CopyTo(output, StartOffset + TwiddledMap.GetTwiddledOffset(x2, y2));

                                offset += (GetBpp(PixelCodec) / 8);
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Small Vq
        // Small Vq
        public class SmallVq : PvrDataCodec
        {
            public override bool CanDecode() { return true;  }
            public override bool CanEncode() { return false; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return 8; }
            public override int GetNumClutEntries() { return 512; }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                int StartOffset = offset;
                byte[] output   = new byte[width * height * 4];
                byte[,] clut    = ClutData;

                TwiddledMap TwiddledMap = new TwiddledMap(width / 2, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += 2)
                {
                    for (int x = 0; x < width; x += 2)
                    {
                        ushort entry = (ushort)((input[StartOffset + TwiddledMap.GetTwiddledOffset(x >> 1, y >> 1)] & 0x7F) * 4);

                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            for (int x2 = 0; x2 < 2; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = clut[entry + (x2 * 2) + y2, 3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = clut[entry + (x2 * 2) + y2, 2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = clut[entry + (x2 * 2) + y2, 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = clut[entry + (x2 * 2) + y2, 0];
                            }
                        }

                        offset++;
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] data, int width, int height, VrPixelCodec PixelCodec)
            {
                return null;
            }
        }
        #endregion

        #region Small Vq with Mipmaps
        // Small Vq with Mipmaps
        public class SmallVqMipmaps : PvrDataCodec
        {
            public override bool CanDecode() { return true;  }
            public override bool CanEncode() { return false; }
            public override int GetBpp(VrPixelCodec PixelCodec) { return 8; }
            public override int GetNumClutEntries() { return 256; }
            public override bool ContainsMipmaps() { return true; }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                int StartOffset = offset;
                byte[] output   = new byte[width * height * 4];
                byte[,] clut    = ClutData;

                TwiddledMap TwiddledMap = new TwiddledMap(width / 2, GetBpp(PixelCodec));

                for (int y = 0; y < height; y += 2)
                {
                    for (int x = 0; x < width; x += 2)
                    {
                        ushort entry = (ushort)((input[StartOffset + TwiddledMap.GetTwiddledOffset(x >> 1, y >> 1)] & 0x3F) * 4);

                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            for (int x2 = 0; x2 < 2; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = clut[entry + (x2 * 2) + y2, 3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = clut[entry + (x2 * 2) + y2, 2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = clut[entry + (x2 * 2) + y2, 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = clut[entry + (x2 * 2) + y2, 0];
                            }
                        }

                        offset++;
                    }
                }

                return output;
            }

            public override byte[] DecodeMipmap(byte[] input, int offset, int mipmap, int width, int height, VrPixelCodec PixelCodec)
            {
                // Get the width of the mipmap and go to the correct offset
                int MipmapWidth = width;
                for (int i = 0; i < mipmap; i++)
                    MipmapWidth >>= 1;

                for (int i = 1; i < MipmapWidth; i <<= 1)
                    offset += (Math.Max(i * i, 4) >> 2);

                return Decode(input, offset, MipmapWidth, MipmapWidth, PixelCodec);
            }

            public override byte[] Encode(byte[] data, int width, int height, VrPixelCodec PixelCodec)
            {
                return null;
            }
        }
        #endregion

        #region Twiddle Code
        // Twiddle Map (its quicker to use recursive functions and build a map)
        private class TwiddledMap
        {
            int[,] map;
            int pos = 0;
            int bpp = 0;

            public TwiddledMap(int size, int bpp)
            {
                map      = new int[size, size];
                this.bpp = bpp;

                if (bpp == 4)
                    BuildMap4(map, 0, 0, size);
                else
                    BuildMap(map, 0, 0, size);
            }

            private void BuildMap(int[,] map, int x, int y, int size)
            {
                if (size == 1)
                {
                    map[x, y] = pos;
                    pos += (bpp / 8);
                }
                else
                {
                    size >>= 1; // Identical to size /= 2 and very slightly faster.
                    BuildMap(map, x, y, size);
                    BuildMap(map, x, y + size, size);
                    BuildMap(map, x + size, y, size);
                    BuildMap(map, x + size, y + size, size);
                }
            }

            private void BuildMap4(int[,] map, int x, int y, int size)
            {
                if (size == 1)
                {
                    map[x, y] = pos >> 1;
                    pos++;
                }
                else
                {
                    size >>= 1; // Identical to size /= 2 and very slightly faster.
                    BuildMap4(map, x, y, size);
                    BuildMap4(map, x, y + size, size);
                    BuildMap4(map, x + size, y, size);
                    BuildMap4(map, x + size, y + size, size);
                }
            }

            public int GetTwiddledOffset(int x, int y)
            {
                return map[x, y];
            }
        }
        #endregion
    }
}