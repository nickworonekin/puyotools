using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace VrSharp
{
    public abstract class VrTexture
    {
        #region Fields
        protected bool initalized = false; // Is the texture initalized?

        protected byte[] encodedData; // Encoded texture data (VR data)

        protected VrPixelCodec pixelCodec; // Pixel codec
        protected VrDataCodec dataCodec;   // Data codec

        protected int paletteOffset; // Offset of the palette data in the texture (-1 if there is none)
        protected int dataOffset;    // Offset of the actual data in the texture
        #endregion

        #region Texture Properties
        /// <summary>
        /// The texture's global index, or 0 if this texture does not have a global index defined.
        /// </summary>
        public uint GlobalIndex
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return globalIndex;
            }
        }
        protected uint globalIndex;

        /// <summary>
        /// Width of the texture (in pixels).
        /// </summary>
        public ushort TextureWidth
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return textureWidth;
            }
        }
        protected ushort textureWidth;

        /// <summary>
        /// Height of the texture (in pixels).
        /// </summary>
        public ushort TextureHeight
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return textureHeight;
            }
        }
        protected ushort textureHeight;

        /// <summary>
        /// Offset of the GBIX (or GCIX) chunk in the texture file, or -1 if this chunk is not present.
        /// </summary>
        public int GbixOffset
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return gbixOffset;
            }
        }
        protected int gbixOffset;

        /// <summary>
        /// Offset of the PVRT (or GVRT) chunk in the texture file.
        /// </summary>
        public int PvrtOffset
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return pvrtOffset;
            }
        }
        protected int pvrtOffset;
        #endregion

        #region Constructors & Initalizers
        // Open a texture from a file.
        public VrTexture(string file)
        {
            encodedData = File.ReadAllBytes(file);

            if (encodedData != null)
            {
                initalized = Initalize();
            }
        }

        // Open a texture from a byte array.
        public VrTexture(byte[] source) : this(source, 0, source.Length) { }

        public VrTexture(byte[] source, int offset, int length)
        {
            if (source == null || (offset == 0 && source.Length == length))
            {
                encodedData = source;
            }
            else if (source != null)
            {
                encodedData = new byte[length];
                Array.Copy(source, offset, encodedData, 0, length);
            }

            if (encodedData != null)
            {
                initalized = Initalize();
            }
        }

        // Open a texture from a stream.
        public VrTexture(Stream source) : this(source, (int)(source.Length - source.Position)) { }

        public VrTexture(Stream source, int length)
        {
            encodedData = new byte[length];
            source.Read(encodedData, 0, length);

            if (encodedData != null)
            {
                initalized = Initalize();
            }
        }

        protected abstract bool Initalize();

        /// <summary>
        /// Returns if the texture was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool Initalized
        {
            get { return initalized; }
        }
        #endregion

        #region Get Texture
        /// <summary>
        /// Returns the decoded texture as an array containg raw 32-bit ARGB data.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            return DecodeTexture();
        }

        /// <summary>
        /// Returns the decoded texture as a bitmap.
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            byte[] data = DecodeTexture();

            Bitmap img = new Bitmap(TextureWidth, TextureHeight, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, img.PixelFormat);
            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
            img.UnlockBits(bitmapData);

            return img;
        }

        /// <summary>
        /// Returns the decoded texture as a stream containg a PNG.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            MemoryStream destination = new MemoryStream();
            ToBitmap().Save(destination, ImageFormat.Png);

            return destination;
        }

        /// <summary>
        /// Saves the decoded texture to the specified file.
        /// </summary>
        /// <param name="file">Name of the file to save the data to.</param>
        public void Save(string file)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            ToBitmap().Save(file, ImageFormat.Png);
        }

        /// <summary>
        /// Saves the decoded texture to the specified stream.
        /// </summary>
        /// <param name="destination">The stream to save the texture to.</param>
        public void Save(Stream destination)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot decode this texture as it is not initalized.");
            }

            ToBitmap().Save(destination, ImageFormat.Png);
        }
        #endregion

        #region Get Texture Mipmap
        /*
        /// <summary>
        /// Get a mipmap in the texture as a byte array (clone of GetTextureMipmapAsArray).
        /// </summary>
        /// <param name="mipmap">Mipmap Level (0 = Largest)</param>
        /// <returns></returns>
        public byte[] GetTextureMipmap(int mipmap)
        {
            return GetTextureMipmapAsArray(mipmap);
        }

        /// <summary>
        /// Get a mipmap in the texture as a byte array.
        /// </summary>
        /// <param name="mipmap">Mipmap Level (0 = Largest)</param>
        /// <returns></returns>
        public byte[] GetTextureMipmapAsArray(int mipmap)
        {
            if (!InitSuccess) return null;

            int size;
            byte[] TextureMipmap = DecodeTextureMipmap(mipmap, out size);
            return ConvertRawToArray(TextureMipmap, size, size);
        }

        /// <summary>
        /// Get a mipmap in the texture as a memory stream.
        /// </summary>
        /// <param name="mipmap">Mipmap Level (0 = Largest)</param>
        /// <returns></returns>
        public MemoryStream GetTextureMipmapAsStream(int mipmap)
        {
            if (!InitSuccess) return null;

            int size;
            byte[] TextureMipmap = DecodeTextureMipmap(mipmap, out size);
            return ConvertRawToStream(TextureMipmap, size, size);
        }

        /// <summary>
        /// Get a mipmap in the texture as a System.Drawing.Bitmap object.
        /// </summary>
        /// <param name="mipmap">Mipmap Level (0 = Largest)</param>
        /// <returns></returns>
        public Bitmap GetTextureMipmapAsBitmap(int mipmap)
        {
            if (!InitSuccess) return null;

            int size;
            byte[] TextureMipmap = DecodeTextureMipmap(mipmap, out size);
            return ConvertRawToBitmap(TextureMipmap, size, size);
        }*/

        /// <summary>
        /// Returns if the texture contains mipmaps.
        /// </summary>
        /// <returns></returns>
        public virtual bool ContainsMipmaps
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return dataCodec.ContainsMipmaps;
            }
        }

        /*
        /// <summary>
        /// Returns the number of mipmaps in the texture, or 0 if there are none.
        /// </summary>
        /// <returns></returns>
        public int GetNumMipmaps()
        {
            if (!InitSuccess)       return 0;
            if (!ContainsMipmaps()) return 0;

            return (int)Math.Log(TextureWidth, 2) + 1;
        }
         */
        #endregion

        #region Palette
        /// <summary>
        /// Set the palette data from an external palette file.
        /// </summary>
        /// <param name="palette">A VpPalette object</param>
        protected virtual void SetPalette(VpPalette palette)
        {
            if (!initalized)
            {
                throw new TextureNotInitalizedException("Cannot set the palette for this texture as it is not initalized.");
            }

            // No need to set an external palette if this data format doesn't require one.
            // We can't just call the data codec here as the data format does not determine
            // if a GVR uses an external palette.
            if (!NeedsExternalPalette)
            {
                return;
            }

            // If the palette is not initalized, don't use it
            if (!palette.Initalized)
            {
                return;
            }

            dataCodec.PixelCodec = palette.PixelCodec;
            dataCodec.SetPalette(palette.EncodedData, 0x10, palette.PaletteEntries);
        }

        /// <summary>
        /// Returns if the texture needs an external palette file.
        /// </summary>
        /// <returns></returns>
        public virtual bool NeedsExternalPalette
        {
            get
            {
                if (!initalized)
                {
                    throw new TextureNotInitalizedException("Cannot access this property as the texture is not initalized.");
                }

                return dataCodec.NeedsExternalPalette;
            }
        }
        #endregion

        #region Private Properties
        // Decode a texture that does not contain mipmaps
        private byte[] DecodeTexture()
        {
            if (paletteOffset != -1) // The texture contains an embedded palette
            {
                dataCodec.SetPalette(encodedData, paletteOffset, dataCodec.PaletteEntries);
            }

            //if (ContainsMipmaps) // If the texture contains mipmaps we have to get the largest texture
            //    return dataCodec.DecodeMipmap(encodedData, dataOffset, 0, textureWidth, textureHeight, pixelCodec);

            return dataCodec.Decode(encodedData, dataOffset, textureWidth, textureHeight, pixelCodec);
        }

        // Decode a texture that contains mipmaps
        /*
        private byte[] DecodeTextureMipmap(int mipmap, out int size)
        {
            if (!ContainsMipmaps()) // No mipmaps = no texture
            {
                size = 0;
                return null;
            }

            // Get the size of the mipmap
            size = TextureWidth;
            for (int i = 0; i < mipmap; i++)
                size >>= 1;
            if (size == 0) // Mipmap > number of mipmaps
                return null;

            if (PaletteOffset != -1) // The texture contains a clut
                DataCodec.SetClut(TextureData, PaletteOffset, PixelCodec);

            return DataCodec.DecodeMipmap(TextureData, DataOffset, mipmap, size, size, PixelCodec);
        }
         */
        #endregion
    }
}