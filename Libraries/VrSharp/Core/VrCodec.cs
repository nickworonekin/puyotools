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
        public abstract byte[,] GetClut(byte[] input, int offset, int entries);
        // Create the clut for this pixel format
        public virtual byte[] CreateClut(byte[,] input) { return null; }
        // Converts the entry for the pixel format to Argb8888
        public virtual byte[] GetPixelPalette(byte[] input, int offset)
        {
            return null;
        }

        // Converts the entry from Argb8888 to the pixel format entry
        public virtual byte[] CreatePixelPalette(byte[] input, int offset)
        {
            return null;
        }
    }
    #endregion

    #region Data Codec
    // Base codec for the data codecs
    public abstract class VrDataCodec : VrCodec
    {
        public abstract bool CanDecode(); // Returns if this format can be decoded
        public abstract bool CanEncode(); // Returns if this format can be encoded

        public abstract int GetBpp(VrPixelCodec PixelCodec); // Returns the bits per pixel for this pixel codec

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

        protected byte[,] ClutData; // Clut for the current texture
        // Set the clut from an external file
        public void SetClutExternal(byte[] clut, int entries, VrPixelCodec PixelCodec)
        {
            ClutData = PixelCodec.GetClut(clut, 0x00, entries);
        }
        // Set the clut
        public void SetClut(byte[] clut, int offset, VrPixelCodec PixelCodec)
        {
            ClutData = PixelCodec.GetClut(clut, offset, GetNumClutEntries());
        }
    }
    #endregion
}