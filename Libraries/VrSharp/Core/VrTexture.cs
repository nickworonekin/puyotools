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
        protected bool InitSuccess = false; // Initalization

        protected byte[] TextureData;  // Vr Texture Data
        protected byte[] RawImageData; // Raw Image Data

        //public uint GlobalIndex { get; protected set; } // Vr Texture Global Index

        //public ushort TextureWidth { get; protected set; }  // Vr Texture Width
        //public ushort TextureHeight { get; protected set; } // Vr Texture Height

        //protected byte PixelFormat;        // Pixel Format
        //protected byte DataFormat;         // Data Format
        protected VrPixelCodec PixelCodec; // Pixel Codec
        protected VrDataCodec DataCodec;   // Data Codec

        //protected int GbixOffset; // Gbix Offset
        //public int PvrtOffset { get; protected set; } // Pvrt (Gvrt) Offset
        protected int ClutOffset; // Clut Offset
        protected int DataOffset; // Data Offset
        #endregion

        #region Constructors
        /*
        /// <summary>
        /// Open a Vr texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public VrTexture(string file)
        {
            byte[] data;
            try
            {
                data = File.ReadAllBytes(file);
            }
            catch { data = new byte[0]; }

            TextureData = data;
        }

        /// <summary>
        /// Open a Vr texture from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the texture data.</param>
        public VrTexture(Stream stream) : this(stream, (int)(stream.Length - stream.Position)) { }

        /// <summary>
        /// Open a Vr texture from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public VrTexture(Stream stream, int length)
        {
            byte[] data;
            try
            {
                data = new byte[length];
                stream.Read(data, 0, length);
            }
            catch { data = new byte[0]; }

            TextureData = data;
        }

        /// <summary>
        /// Open a Vr texture from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the texture data.</param>
        public VrTexture(byte[] array) : this(array, 0, array.Length) { }

        /// <summary>
        /// Open a Vr texture from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public VrTexture(byte[] array, long offset, int length)
        {
            byte[] data;
            if (array == null)
                data = new byte[0];
            else
            {
                data = new byte[length];
                try
                {
                    Array.Copy(array, offset, data, 0, length);
                }
                catch { data = new byte[0]; }
            }

            TextureData = data;
        }
         * */
        #endregion

        #region Get Texture
        /// <summary>
        /// Get the texture as a byte array (clone of GetTextureAsArray).
        /// </summary>
        /// <returns></returns>
        public byte[] GetTexture()
        {
            return GetTextureAsArray();
        }

        /// <summary>
        /// Get the texture as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetTextureAsArray()
        {
            if (!InitSuccess) return null;

            return ConvertRawToArray(DecodeTexture(), TextureWidth, TextureHeight);
        }

        /// <summary>
        /// Get the texture as a memory stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetTextureAsStream()
        {
            if (!InitSuccess) return null;

            return ConvertRawToStream(DecodeTexture(), TextureWidth, TextureHeight);
        }

        /// <summary>
        /// Get the texture as a System.Drawing.Bitmap object.
        /// </summary>
        /// <returns></returns>
        public Bitmap GetTextureAsBitmap()
        {
            if (!InitSuccess) return null;

            return ConvertRawToBitmap(DecodeTexture(), TextureWidth, TextureHeight);
        }
        #endregion

        #region Get Texture Mipmap
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
        }

        /// <summary>
        /// Returns if the texture contains mipmaps.
        /// </summary>
        /// <returns></returns>
        public virtual bool ContainsMipmaps()
        {
            if (!InitSuccess) return false;
            return DataCodec.ContainsMipmaps();
        }

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
        #endregion

        #region Clut
        /// <summary>
        /// Set the clut data from an external clut file.
        /// </summary>
        /// <param name="clut">A VpClut object</param>
        public virtual void SetClut(VpClut clut)
        {
            // Should throw an ArgumentException if not the right type of
            // VrClut (ex: passing a PvpClut for a GvrTexture).

            if (!InitSuccess) return;
            if (!NeedsExternalClut()) return; // Can't use DataCodec here

            if (clut.PixelCodec != null)
                DataCodec.SetClutExternal(clut.GetClut(clut.PixelCodec), clut.GetNumClutEntries(), clut.PixelCodec);
            else
                DataCodec.SetClutExternal(clut.GetClut(PixelCodec), clut.GetNumClutEntries(), PixelCodec);
        }

        /// <summary>
        /// Returns if the texture needs an external clut file.
        /// </summary>
        /// <returns></returns>
        public virtual bool NeedsExternalClut()
        {
            if (!InitSuccess) return false;

            return DataCodec.NeedsExternalClut();
        }
        #endregion

        #region Misc
        /// <summary>
        /// Returns if the texture was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool LoadSuccess()
        {
            return InitSuccess;
        }

        /*
        /// <summary>
        /// Returns information about the texture.
        /// </summary>
        /// <returns></returns>
        public abstract VrTextureInfo GetTextureInfo();
         * */
        #endregion

        #region Private Properties
        // Convert raw image data to a bitmap
        private Bitmap ConvertRawToBitmap(byte[] input, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(input, 0, bitmapData.Scan0, input.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        // Convert raw image data to a memory stream
        private MemoryStream ConvertRawToStream(byte[] input, int width, int height)
        {
            Bitmap bitmap       = ConvertRawToBitmap(input, width, height);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);

            return stream;
        }

        // Convert raw image data to a byte array
        private byte[] ConvertRawToArray(byte[] input, int width, int height)
        {
            return ConvertRawToStream(input, width, height).ToArray();
        }

        // Decode a texture that does not contain mipmaps
        private byte[] DecodeTexture()
        {
            if (ClutOffset != -1) // The texture contains a clut
                DataCodec.SetClut(TextureData, ClutOffset, PixelCodec);

            if (ContainsMipmaps()) // If the texture contains mipmaps we have to get the largest texture
                return DataCodec.DecodeMipmap(TextureData, DataOffset, 0, TextureWidth, TextureHeight, PixelCodec);

            return DataCodec.Decode(TextureData, DataOffset, TextureWidth, TextureHeight, PixelCodec);
        }

        // Decode a texture that contains mipmaps
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

            if (ClutOffset != -1) // The texture contains a clut
                DataCodec.SetClut(TextureData, ClutOffset, PixelCodec);

            return DataCodec.DecodeMipmap(TextureData, DataOffset, mipmap, size, size, PixelCodec);
        }

        // Function for checking headers
        // Checks to see if the string matches the byte data at the specific offset
        protected static bool Compare(byte[] array, string str, int offset)
        {
            if (offset < 0 || offset + str.Length > array.Length)
                return false; // Out of bounds

            for (int i = 0; i < str.Length; i++)
            {
                if (array[offset + i] != str[i])
                    return false;
            }

            return true;
        }
        #endregion

        #region Texture Properties
        /// <summary>
        /// The texture's global index, or 0 if this texture does not have a global index defined.
        /// </summary>
        public uint GlobalIndex { get; protected set; }

        /// <summary>
        /// Width of the texture (in pixels).
        /// </summary>
        public ushort TextureWidth { get; protected set; }

        /// <summary>
        /// Height of the texture (in pixels).
        /// </summary>
        public ushort TextureHeight { get; protected set; }

        /// <summary>
        /// Offset of the GBIX (or GCIX) chunk in the texture file, or -1 if this chunk is not present.
        /// </summary>
        public int GbixOffset { get; protected set; }

        /// <summary>
        /// Offset of the PVRT (or GVRT) chunk in the texture file.
        /// </summary>
        public int PvrtOffset { get; protected set; }
        #endregion

        #region Constructors & Initalizers
        public VrTexture(string file)
        {
            try
            {
                TextureData = File.ReadAllBytes(file);
            }
            catch
            {
                TextureData = null;
            }

            if (TextureData != null)
            {
                InitSuccess = Initalize();
            }
            else
            {
                InitSuccess = false;
            }
        }

        public VrTexture(byte[] source)
        {
            TextureData = source;

            if (TextureData != null)
            {
                InitSuccess = Initalize();
            }
            else
            {
                InitSuccess = false;
            }
        }

        public VrTexture(byte[] source, int offset, int length)
        {
            if (source == null || (offset == 0 && source.Length == length))
            {
                TextureData = source;
            }
            else if (source != null)
            {
                TextureData = new byte[length];
                Array.Copy(source, offset, TextureData, 0, length);
            }

            if (TextureData != null)
            {
                InitSuccess = Initalize();
            }
            else
            {
                InitSuccess = false;
            }
        }

        public VrTexture(Stream source) : this(source, (int)(source.Length - source.Position)) { }

        public VrTexture(Stream source, int length)
        {
            try
            {
                TextureData = new byte[length];
                source.Read(TextureData, 0, length);
            }
            catch
            {
                TextureData = null;
            }

            if (TextureData != null)
            {
                InitSuccess = Initalize();
            }
            else
            {
                InitSuccess = false;
            }
        }

        protected abstract bool Initalize();
        #endregion
    }
}