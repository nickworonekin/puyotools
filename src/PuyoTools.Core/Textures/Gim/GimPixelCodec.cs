using System;
using System.Drawing;

namespace GimSharp
{
    public abstract class GimPixelCodec
    {
        #region Pixel Codec
        // Returns if we can encode using this codec.
        public abstract bool CanEncode { get; }

        // Returns the bits per pixel for this pixel format.
        public abstract int Bpp { get; }

        // Decode & Encode a pixel
        public abstract void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex);
        public abstract void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex);

        // Decode & Encode a palette
        public byte[][] DecodePalette(byte[] source, int sourceIndex, int numEntries)
        {
            byte[][] palette = new byte[numEntries][];

            for (int i = 0; i < numEntries; i++)
            {
                palette[i] = new byte[4];
                DecodePixel(source, sourceIndex + (i * (Bpp >> 3)), palette[i], 0);
            }

            return palette;
        }

        public byte[] EncodePalette(byte[][] palette, int numEntries)
        {
            byte[] destination = new byte[numEntries * (Bpp >> 3)];
            int destinationIndex = 0;

            for (int i = 0; i < numEntries; i++)
            {
                EncodePixel(palette[i], 0, destination, destinationIndex);
                destinationIndex += (Bpp >> 3);
            }

            return destination;
        }
        #endregion

        #region Rgb565
        // Rgb565
        public class Rgb565 : GimPixelCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 16; }
            }

            public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

                destination[destinationIndex + 3] = 0xFF;
                destination[destinationIndex + 2] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 5)  & 0x3F) * 0xFF / 0x3F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 0);
                pixel |= (ushort)((source[sourceIndex + 1] >> 2) << 5);
                pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 11);

                destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
            }
        }
        #endregion

        #region Argb1555
        // Argb1555
        public class Argb1555 : GimPixelCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 16; }
            }

            public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

                destination[destinationIndex + 3] = (byte)(((pixel >> 15) & 0x01) * 0xFF);
                destination[destinationIndex + 2] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 5)  & 0x1F) * 0xFF / 0x1F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)((source[sourceIndex + 3] >> 7) << 15);
                pixel |= (ushort)((source[sourceIndex + 2] >> 3) << 0);
                pixel |= (ushort)((source[sourceIndex + 1] >> 3) << 5);
                pixel |= (ushort)((source[sourceIndex + 0] >> 3) << 10);

                destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
            }
        }
        #endregion

        #region Argb4444
        // Argb4444
        public class Argb4444 : GimPixelCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 16; }
            }

            public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = BitConverter.ToUInt16(source, sourceIndex);

                destination[destinationIndex + 3] = (byte)(((pixel >> 12) & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 2] = (byte)(((pixel >> 0)  & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 1] = (byte)(((pixel >> 4)  & 0x0F) * 0xFF / 0x0F);
                destination[destinationIndex + 0] = (byte)(((pixel >> 8)  & 0x0F) * 0xFF / 0x0F);
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                ushort pixel = 0x0000;
                pixel |= (ushort)((source[sourceIndex + 3] >> 4) << 12);
                pixel |= (ushort)((source[sourceIndex + 2] >> 4) << 0);
                pixel |= (ushort)((source[sourceIndex + 1] >> 4) << 4);
                pixel |= (ushort)((source[sourceIndex + 0] >> 4) << 8);

                destination[destinationIndex + 1] = (byte)((pixel >> 8) & 0xFF);
                destination[destinationIndex + 0] = (byte)(pixel & 0xFF);
            }
        }
        #endregion

        #region Argb8888
        // Argb8888
        public class Argb8888 : GimPixelCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 32; }
            }

            public override void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                destination[destinationIndex + 3] = source[sourceIndex + 3];
                destination[destinationIndex + 2] = source[sourceIndex + 0];
                destination[destinationIndex + 1] = source[sourceIndex + 1];
                destination[destinationIndex + 0] = source[sourceIndex + 2];
            }

            public override void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex)
            {
                destination[destinationIndex + 3] = source[sourceIndex + 3];
                destination[destinationIndex + 2] = source[sourceIndex + 0];
                destination[destinationIndex + 1] = source[sourceIndex + 1];
                destination[destinationIndex + 0] = source[sourceIndex + 2];
            }
        }
        #endregion

        #region Get Codec
        // Only used for palettized formats
        public static GimPixelCodec GetPixelCodec(GimPaletteFormat format)
        {
            switch (format)
            {
                case GimPaletteFormat.Rgb565:
                    return new Rgb565();
                case GimPaletteFormat.Argb1555:
                    return new Argb1555();
                case GimPaletteFormat.Argb4444:
                    return new Argb4444();
                case GimPaletteFormat.Argb8888:
                    return new Argb8888();
            }

            return null;
        }
        #endregion
    }
}