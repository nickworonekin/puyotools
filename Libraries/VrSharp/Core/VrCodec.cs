using System;

namespace VrSharp
{
    #region Base Codec
    // Base codec for all codecs
    public abstract class VrCodec
    {
        // Swap endian of a 16-bit unsigned integer (a ushort)
        protected ushort SwapUShort(ushort x)
        {
            return (ushort)((x << 8) | (x >> 8));
        }
    }
    #endregion

    #region Pixel Codec
    // Base codec for the pixel codecs
    public abstract class VrPixelCodec : VrCodec
    {
        public abstract bool CanDecode(); // Returns if this format can be decoded
        public abstract bool CanEncode(); // Returns if this format can be encoded

        public abstract int GetBpp(); // Returns the bits per pixel for this pixel codec

        // Gets the clut for this pixel format and returns it as Argb8888
        //public abstract byte[,] GetClut(byte[] input, int offset, int entries);
        // Create the clut for this pixel format
        //public virtual byte[] CreateClut(byte[,] input) { return null; }
        // Converts the entry for the pixel format to Argb8888
        //public virtual byte[] GetPixelPalette(byte[] input, int offset)
        //{
        //    return null;
        //}

        // Converts the entry from Argb8888 to the pixel format entry
        //public virtual byte[] CreatePixelPalette(byte[] input, int offset)
        //{
        //    return null;
        //}

        public abstract int Bpp { get; }

        public abstract void DecodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex);
        public abstract void EncodePixel(byte[] source, int sourceIndex, byte[] destination, int destinationIndex);

        public byte[] EncodeClut(byte[][] palette, int numEntries)
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

        public byte[][] DecodeClut(byte[] source, int sourceIndex, int numEntries)
        {
            byte[][] palette = new byte[numEntries][];

            for (int i = 0; i < numEntries; i++)
            {
                palette[i] = new byte[4];
                DecodePixel(source, sourceIndex + (i * (Bpp >> 3)), palette[i], 0);
            }

            return palette;
        }
    }
    #endregion

    #region Data Codec
    // Base codec for the data codecs
    public abstract class VrDataCodec : VrCodec
    {
        public VrPixelCodec PixelCodec;

        public abstract bool CanDecode(); // Returns if this format can be decoded
        public abstract bool CanEncode(); // Returns if this format can be encoded

        public abstract int GetBpp(VrPixelCodec PixelCodec); // Returns the bits per pixel for this pixel codec

        public virtual int Bpp
        {
            get { return 0; }
        }

        public virtual int ClutEntries
        {
            get { return 0; }
        }

        // Returns the number of entries in the clut (0 if there is no clut)
        public virtual int GetNumClutEntries() { return 0; }
        // Returns if the format requires an external clut file
        public virtual bool NeedsExternalClut() { return false; }
        // Returns if the texture contains mipmaps
        public virtual bool ContainsMipmaps() { return false; }

        // Decode texture data
        public abstract byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec);
        // Decode a mipmap in the texture data
        public virtual byte[] DecodeMipmap(byte[] input, int offset, int mipmap, int width, int height, VrPixelCodec PixelCodec)
        {
            return Decode(input, offset, width, height, PixelCodec);
        }
        // Encode texture data
        public abstract byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec);

        protected byte[][] ClutData;

        //protected byte[,] ClutData; // Clut for the current texture
        // Set the clut from an external file
        public void SetClutExternal(byte[] clut, int entries, VrPixelCodec PixelCodec)
        {
            //ClutData = PixelCodec.GetClut(clut, 0x00, entries);
            ClutData = PixelCodec.DecodeClut(clut, 0, entries);
        }
        // Set the clut
        public void SetClut(byte[] clut, int offset, VrPixelCodec PixelCodec)
        {
            //ClutData = PixelCodec.GetClut(clut, offset, GetNumClutEntries());
            ClutData = PixelCodec.DecodeClut(clut, offset, GetNumClutEntries());
        }
    }
    #endregion
}