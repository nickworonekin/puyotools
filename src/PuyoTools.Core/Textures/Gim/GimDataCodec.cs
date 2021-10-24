using System;

namespace PuyoTools.Core.Textures.Gim
{
    public abstract class GimDataCodec
    {
        #region Data Codec
        // The pixel codec to use for this data codec.
        public GimPixelCodec PixelCodec;

        // Returns if we can encode using this codec.
        public abstract bool CanEncode { get; }

        // Returns the bits per pixel for this data format.
        public abstract int Bpp { get; }

        // Returns the number of palette entries for this data format.
        // Returns 0 if this is not a palettized data format.
        public virtual int PaletteEntries
        {
            get { return 0; }
        }

        // Palette
        protected byte[][] palette;
        public void SetPalette(byte[] palette, int offset, int numEntries)
        {
            this.palette = PixelCodec.DecodePalette(palette, offset, numEntries);
        }

        // Decode & Encode texture data
        public abstract byte[] Decode(byte[] source, int sourceIndex, int width, int height);
        public abstract byte[] Encode(byte[] source, int sourceIndex, int width, int height);
        #endregion

        #region Argb Base (for Rgb565, Argb1555, Argb4444, and Argb8888)
        // ARGB Base (for Rgb565, Argb1555, Argb4444, and Argb8888)
        public abstract class ArgbBase : GimDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override byte[] Decode(byte[] source, int sourceIndex, int width, int height)
            {
                // Destination data & index
                byte[] destination = new byte[width * height * 4];
                int destinationIndex = 0;

                // Decode texture data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.DecodePixel(source, sourceIndex, destination, destinationIndex);
                        sourceIndex += PixelCodec.Bpp / 8;
                        destinationIndex += 4;
                    }
                }

                return destination;
            }

            public override byte[] Encode(byte[] source, int sourceIndex, int width, int height)
            {
                // Destination data & index
                byte[] destination = new byte[width * height * (PixelCodec.Bpp >> 3)];
                int destinationIndex = 0;

                // Encode texture data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.EncodePixel(source, sourceIndex, destination, destinationIndex);
                        sourceIndex += 4;
                        destinationIndex += PixelCodec.Bpp / 8;
                    }
                }

                return destination;
            }
        }
        #endregion

        #region Rgb565
        // Rgb565
        public class Rgb565 : ArgbBase
        {
            public override int Bpp
            {
                get { return 16; }
            }

            public Rgb565()
            {
                PixelCodec = new GimPixelCodec.Rgb565();
            }
        }
        #endregion

        #region Argb1555
        // Argb1555
        public class Argb1555 : ArgbBase
        {
            public override int Bpp
            {
                get { return 16; }
            }

            public Argb1555()
            {
                PixelCodec = new GimPixelCodec.Argb1555();
            }
        }
        #endregion

        #region Argb4444
        // Argb4444
        public class Argb4444 : ArgbBase
        {
            public override int Bpp
            {
                get { return 16; }
            }

            public Argb4444()
            {
                PixelCodec = new GimPixelCodec.Argb4444();
            }
        }
        #endregion

        #region Argb8888
        // Argb8888
        public class Argb8888 : ArgbBase
        {
            public override int Bpp
            {
                get { return 32; }
            }

            public Argb8888()
            {
                PixelCodec = new GimPixelCodec.Argb8888();
            }
        }
        #endregion

        #region 4-bit Indexed
        // Square Twiddled
        public class Index4 : GimDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 4; }
            }

            public override int PaletteEntries
            {
                get { return 16; }
            }

            public override byte[] Decode(byte[] source, int sourceIndex, int width, int height)
            {
                // Destination data & index
                byte[] destination = new byte[width * height * 4];
                int destinationIndex = 0;

                // Decode texture data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte index = (byte)((source[sourceIndex] >> ((x & 0x1) * 4)) & 0xF);

                        for (int i = 0; i < 4; i++)
                        {
                            destination[destinationIndex] = palette[index][i];
                            destinationIndex++;
                        }

                        if ((x & 0x1) != 0)
                        {
                            sourceIndex++;
                        }
                    }
                }

                return destination;
            }

            public override byte[] Encode(byte[] source, int sourceIndex, int width, int height)
            {
                // Destination data & index
                byte[] destination = new byte[(width * height) >> 1];
                int destinationIndex = 0;

                // Encode texture data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte index = destination[destinationIndex];
                        index |= (byte)((source[sourceIndex] & 0xF) << ((x & 0x1) * 4));

                        destination[destinationIndex] = index;
                        sourceIndex++;

                        if ((x & 0x1) != 0)
                        {
                            destinationIndex++;
                        }
                    }
                }

                return destination;
            }
        }
        #endregion

        #region 8-bit Indexed
        // Square Twiddled with Mipmaps
        public class Index8 : GimDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 8; }
            }

            public override int PaletteEntries
            {
                get { return 256; }
            }

            public override byte[] Decode(byte[] source, int sourceIndex, int width, int height)
            {
                // Destination data & index
                byte[] destination = new byte[width * height * 4];
                int destinationIndex = 0;

                // Decode texture data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte index = source[sourceIndex];

                        for (int i = 0; i < 4; i++)
                        {
                            destination[destinationIndex] = palette[index][i];
                            destinationIndex++;
                        }

                        sourceIndex++;
                    }
                }

                return destination;
            }

            public override byte[] Encode(byte[] source, int sourceIndex, int width, int height)
            {
                // Destination data & index
                byte[] destination = new byte[width * height];
                int destinationIndex = 0;

                // Encode texture data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        destination[destinationIndex] = source[sourceIndex];
                        sourceIndex++;
                        destinationIndex++;
                    }
                }

                return destination;
            }
        }
        #endregion

        #region Swizzle Code
        // Swizzle the texture to the PSP format
        public static byte[] Swizzle(byte[] source, int offset, int width, int height, int bpp)
        {
            // Incorperate the bpp into the width
            width = (width * bpp) >> 3;

            byte[] destination = new byte[width * height];

            int rowblocks = (width / 16);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int blockX = x / 16;
                    int blockY = y / 8;

                    int blockIndex = blockX + ((blockY) * rowblocks);
                    int blockAddress = blockIndex * 16 * 8;

                    destination[blockAddress + (x - blockX * 16) + ((y - blockY * 8) * 16)] = source[offset];
                    offset++;
                }
            }

            return destination;
        }

        // Unswizzle the texture from the PSP format
        public static byte[] UnSwizzle(byte[] source, int offset, int width, int height, int bpp)
        {
            int destinationOffset = 0;

            // Incorperate the bpp into the width
            width = (width * bpp) >> 3;

            byte[] destination = new byte[width * height];

            int rowblocks = (width / 16);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int blockX = x / 16;
                    int blockY = y / 8;

                    int blockIndex = blockX + ((blockY) * rowblocks);
                    int blockAddress = blockIndex * 16 * 8;

                    destination[destinationOffset] = source[offset + blockAddress + (x - blockX * 16) + ((y - blockY * 8) * 16)];
                    destinationOffset++;
                }
            }

            return destination;
        }
        #endregion

        #region Get Codec
        public static GimDataCodec GetDataCodec(GimDataFormat format)
        {
            switch (format)
            {
                case GimDataFormat.Rgb565:
                    return new Rgb565();
                case GimDataFormat.Argb1555:
                    return new Argb1555();
                case GimDataFormat.Argb4444:
                    return new Argb8888();
                case GimDataFormat.Argb8888:
                    return new Argb8888();
                case GimDataFormat.Index4:
                    return new Index4();
                case GimDataFormat.Index8:
                    return new Index8();
            }

            return null;
        }
        #endregion
    }
}