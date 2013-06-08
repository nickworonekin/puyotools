using System;

namespace VrSharp.PvrTexture
{
    public abstract class PvrDataCodec : VrDataCodec
    {
        #region Square Twiddled
        // Square Twiddled
        public class SquareTwiddled : PvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return PixelCodec.Bpp; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int[] twiddleMap = MakeTwiddleMap(width);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.DecodePixel(input, offset + (((twiddleMap[x] << 1) | twiddleMap[y]) << (PixelCodec.Bpp >> 4)), output, (((y * width) + x) * 4));
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * (PixelCodec.Bpp >> 3)];
                int[] twiddleMap = MakeTwiddleMap(width);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.EncodePixel(input, (((y * width) + x) * 4), output, ((twiddleMap[x] << 1) | twiddleMap[y]) << (PixelCodec.Bpp >> 4));
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
            public override bool CanEncode
            {
                get { return false; }
            }

            public override int Bpp
            {
                get { return PixelCodec.Bpp; }
            }

            public override bool ContainsMipmaps
            {
                get { return true; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int[] twiddleMap = MakeTwiddleMap(width);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.DecodePixel(input, offset + (((twiddleMap[x] << 1) | twiddleMap[y]) << (PixelCodec.Bpp >> 4)), output, (((y * width) + x) * 4));
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
                    offset += Math.Max(i * i * (PixelCodec.Bpp >> 3), 4);

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
            public override bool CanEncode
            {
                get { return false; }
            }

            public override int Bpp
            {
                get { return 2; }
            }

            public override int PaletteEntries
            {
                get { return 1024; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int[] twiddleMap = MakeTwiddleMap(width);

                for (int x = 0; x < width; x += 2)
                {
                    for (int y = 0; y < height; y += 2)
                    {
                        int index = input[offset + ((twiddleMap[x >> 1] << 1) | twiddleMap[y >> 1])] << 2;

                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            for (int x2 = 0; x2 < 2; x2++)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    output[((((y + y2) * width) + (x + x2)) * 4) + i] = palette[index + (x2 * 2) + y2][i];
                                }
                            }
                        }
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
            public override bool CanEncode
            {
                get { return false; }
            }

            public override int Bpp
            {
                get { return 2; }
            }

            public override int PaletteEntries
            {
                get { return 1024; }
            }

            public override bool ContainsMipmaps
            {
                get { return true; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int[] twiddleMap = MakeTwiddleMap(width);

                for (int x = 0; x < width; x += 2)
                {
                    for (int y = 0; y < height; y += 2)
                    {
                        int index = input[offset + ((twiddleMap[x >> 1] << 1) | twiddleMap[y >> 1])] << 2;

                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            for (int x2 = 0; x2 < 2; x2++)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    output[((((y + y2) * width) + (x + x2)) * 4) + i] = palette[index + (x2 * 2) + y2][i];
                                }
                            }
                        }
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

        #region 4-bit Indexed with External Palette
        // 4-bit Indexed with External Palette
        public class Index4 : PvrDataCodec
        {
            public override bool CanEncode
            {
                get { return false; }
            }

            public override int Bpp
            {
                get { return 4; }
            }

            public override int PaletteEntries
            {
                get { return 16; }
            }

            public override bool NeedsExternalPalette
            {
                get { return true; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int size = Math.Min(width, height);
                int[] twiddleMap = MakeTwiddleMap(size);

                for (int x = 0; x < width; x += size)
                {
                    for (int y = 0; y < height; y += size)
                    {
                        for (int y2 = 0; y2 < size; y2++)
                        {
                            for (int x2 = 0; x2 < size; x2++)
                            {
                                byte index = (byte)((input[offset + (((twiddleMap[x2] << 1) | twiddleMap[y2]) >> 1)] >> ((y2 & 0x1) * 4)) & 0xF);
                                for (int i = 0; i < 4; i++)
                                {
                                    output[((((y + y2) * width) + (x + x2)) * 4) + i] = palette[index][i];
                                }
                            }
                        }

                        offset += (size * size) >> 1;
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

        #region 8-bit Indexed with External Palette
        // 8-bit Indexed with External Palette
        public class Index8 : PvrDataCodec
        {
            public override bool CanEncode
            {
                get { return false; }
            }

            public override int Bpp
            {
                get { return 8; }
            }

            public override int PaletteEntries
            {
                get { return 256; }
            }

            public override bool NeedsExternalPalette
            {
                get { return true; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int size = Math.Min(width, height);
                int[] twiddleMap = MakeTwiddleMap(size);

                for (int x = 0; x < width; x += size)
                {
                    for (int y = 0; y < height; y += size)
                    {
                        for (int y2 = 0; y2 < size; y2++)
                        {
                            for (int x2 = 0; x2 < size; x2++)
                            {
                                byte index = input[offset + ((twiddleMap[x2] << 1) | twiddleMap[y2])];
                                for (int i = 0; i < 4; i++)
                                {
                                    output[((((y + y2) * width) + (x + x2)) * 4) + i] = palette[index][i];
                                }
                            }
                        }

                        offset += (size * size);
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

        #region Rectangle
        // Rectangle
        public class Rectangle : PvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return PixelCodec.Bpp; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y ++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.DecodePixel(input, offset, output, (((y * width) + x) * 4));

                        offset += PixelCodec.Bpp >> 3;
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset    = 0;
                byte[] output = new byte[width * height * (PixelCodec.Bpp >> 3)];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.EncodePixel(input, (((y * width) + x) * 4), output, offset);

                        offset += PixelCodec.Bpp >> 3;
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
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return PixelCodec.Bpp; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int size = Math.Min(width, height);
                int[] twiddleMap = MakeTwiddleMap(size);

                for (int x = 0; x < width; x += size)
                {
                    for (int y = 0; y < height; y += size)
                    {
                        for (int y2 = 0; y2 < size; y2++)
                        {
                            for (int x2 = 0; x2 < size; x2++)
                            {
                                PixelCodec.DecodePixel(input, offset + (((twiddleMap[x2] << 1) | twiddleMap[y2]) << (PixelCodec.Bpp >> 4)), output, ((((y + y2) * width) + (x + x2)) * 4));
                            }
                        }

                        offset += size * size * (PixelCodec.Bpp >> 3);
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * (PixelCodec.Bpp >> 3)];
                int size = Math.Min(width, height);
                int[] twiddleMap = MakeTwiddleMap(size);

                int offset = 0;

                for (int x = 0; x < width; x += size)
                {
                    for (int y = 0; y < height; y += size)
                    {
                        for (int y2 = 0; y2 < size; y2++)
                        {
                            for (int x2 = 0; x2 < size; x2++)
                            {
                                PixelCodec.EncodePixel(input, ((((y + y2) * width) + (x + x2)) * 4), output, offset + (((twiddleMap[x2] << 1) | twiddleMap[y2]) << (PixelCodec.Bpp >> 4)));
                            }
                        }

                        offset += size * size * (PixelCodec.Bpp >> 3);
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
            public override bool CanEncode
            {
                get { return false; }
            }

            public override int Bpp
            {
                get { return 2; }
            }

            public override int PaletteEntries
            {
                get { return 512; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int[] twiddleMap = MakeTwiddleMap(width);

                for (int x = 0; x < width; x += 2)
                {
                    for (int y = 0; y < height; y += 2)
                    {
                        int index = (input[offset + ((twiddleMap[x >> 1] << 1) | twiddleMap[y >> 1])] & 0x7F) << 2;

                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            for (int x2 = 0; x2 < 2; x2++)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    output[((((y + y2) * width) + (x + x2)) * 4) + i] = palette[index + (x2 * 2) + y2][i];
                                }
                            }
                        }
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
            public override bool CanEncode
            {
                get { return false; }
            }

            public override int Bpp
            {
                get { return 2; }
            }

            public override int PaletteEntries
            {
                get { return 256; }
            }

            public override bool ContainsMipmaps
            {
                get { return true; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                int[] twiddleMap = MakeTwiddleMap(width);

                for (int x = 0; x < width; x += 2)
                {
                    for (int y = 0; y < height; y += 2)
                    {
                        int index = (input[offset + ((twiddleMap[x >> 1] << 1) | twiddleMap[y >> 1])] & 0x3F) << 2;

                        for (int y2 = 0; y2 < 2; y2++)
                        {
                            for (int x2 = 0; x2 < 2; x2++)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    output[((((y + y2) * width) + (x + x2)) * 4) + i] = palette[index + (x2 * 2) + y2][i];
                                }
                            }
                        }
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
        // Makes a twiddle map for the specified size texture
        private int[] MakeTwiddleMap(int size)
        {
            int[] twiddleMap = new int[size];

            for (int i = 0; i < size; i++)
            {
                twiddleMap[i] = 0;

                for (int j = 0, k = 1; k <= i; j++, k <<= 1)
                {
                    twiddleMap[i] |= (i & k) << j;
                }
            }

            return twiddleMap;
        }
        #endregion

        #region Get Codec
        public static PvrDataCodec GetDataCodec(PvrDataFormat format)
        {
            switch (format)
            {
                case PvrDataFormat.SquareTwiddled:
                    return new SquareTwiddled();
                case PvrDataFormat.SquareTwiddledMipmaps:
                case PvrDataFormat.SquareTwiddledMipmapsAlt:
                    return new SquareTwiddledMipmaps();
                case PvrDataFormat.Vq:
                    return new Vq();
                case PvrDataFormat.VqMipmaps:
                    return new VqMipmaps();
                case PvrDataFormat.Index4:
                    return new Index4();
                case PvrDataFormat.Index8:
                    return new Index8();
                case PvrDataFormat.Rectangle:
                    return new Rectangle();
                case PvrDataFormat.RectangleTwiddled:
                    return new RectangleTwiddled();
                case PvrDataFormat.SmallVq:
                    return new SmallVq();
                case PvrDataFormat.SmallVqMipmaps:
                    return new SmallVqMipmaps();
            }

            return null;
        }
        #endregion
    }
}