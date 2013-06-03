using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace VrSharp.GvrTexture
{
    public class GvrTextureEncoder : VrTextureEncoder
    {
        #region Fields
        //public GvrPixelFormat PixelFormat { get; private set; }
        //public GvrDataFormat DataFormat { get; private set; }
        //public GvrDataFlags DataFlags { get; private set; } // Data Flags
        #endregion

        #region Constructors
        /*
        /// <summary>
        /// Open a bitmap from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the bitmap data.</param>
        /// <param name="PixelFormat">Pixel Format</param>
        /// <param name="DataFormat">Data Format</param>
        public GvrTextureEncoder(string file, GvrPixelFormat PixelFormat, GvrDataFormat DataFormat)
            : base(file)
        {
            this.PixelFormat = PixelFormat;
            this.DataFormat  = DataFormat;

            InitSuccess = Initalize();
        }

        /// <summary>
        /// Open a bitmap from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the bitmap data.</param>
        /// <param name="PixelFormat">Pixel Format</param>
        /// <param name="DataFormat">Data Format</param>
        public GvrTextureEncoder(Stream stream, GvrPixelFormat PixelFormat, GvrDataFormat DataFormat)
            : base(stream)
        {
            this.PixelFormat = PixelFormat;
            this.DataFormat  = DataFormat;

            InitSuccess = Initalize();
        }

        /// <summary>
        /// Open a bitmap from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the bitmap data.</param>
        /// <param name="PixelFormat">Pixel Format</param>
        /// <param name="DataFormat">Data Format</param>
        public GvrTextureEncoder(byte[] array, GvrPixelFormat PixelFormat, GvrDataFormat DataFormat)
            : base(array)
        {
            this.PixelFormat = PixelFormat;
            this.DataFormat  = DataFormat;

            InitSuccess = Initalize();
        }

        /// <summary>
        /// Open a bitmap from a System.Drawing.Bitmap.
        /// </summary>
        /// <param name="bitmap">A System.Drawing.Bitmap instance.</param>
        /// <param name="PixelFormat">Pixel Format</param>
        /// <param name="DataFormat">Data Format</param>
        public GvrTextureEncoder(Bitmap bitmap, GvrPixelFormat PixelFormat, GvrDataFormat DataFormat)
            : base(bitmap)
        {
            this.PixelFormat = PixelFormat;
            this.DataFormat  = DataFormat;

            InitSuccess = Initalize();
        }
        */
        #endregion

        /*
        #region Enums
        public enum GvrGbixType
        {
            Gamecube,
            Wii,
        }
        #endregion
         * */

        /*
        #region Misc
        /// <summary>
        /// Returns information about the texture. (Use an explicit cast to get GvrTextureInfo.)
        /// </summary>
        /// <returns></returns>
        public override VrTextureInfo GetTextureInfo()
        {
            if (!InitSuccess) return new GvrTextureInfo();

            GvrTextureInfo TextureInfo = new GvrTextureInfo();
            TextureInfo.TextureWidth   = TextureWidth;
            TextureInfo.TextureHeight  = TextureHeight;
            TextureInfo.PixelFormat    = DataFormat;
            TextureInfo.DataFormat     = DataFormat;
            TextureInfo.DataFlags      = DataFlags;

            return TextureInfo;
        }
        #endregion
         */

        #region Clut
        protected override void CreateVpClut(byte[] ClutData, ushort NumClutEntries)
        {
            ClutEncoder = new GvpPaletteEncoder(ClutData, NumClutEntries, (GvrPixelFormat)PixelFormat);
        }
        #endregion

        /*
        /// <summary>
        /// Set data flags.
        /// </summary>
        /// <param name="Mipmaps">Texture contains mipmaps.</param>
        /// <param name="ExternalClut">Palettized texture contains an external clut.</param>
        public void SetDataFlags(bool Mipmaps, bool ExternalClut)
        {
            if (!InitSuccess) return;

            if (Mipmaps && DataCodec.ClutEntries == 0) // No mipmaps for palettized textures yet
                DataFlags |= GvrDataFlags.Mipmaps;
            if (ExternalClut && DataCodec.ClutEntries != 0)
                DataFlags |= GvrDataFlags.ExternalClut;
        }
         * */

        /*
        // Initalize the bitmap
        private bool Initalize()
        {
            // Make sure the width and height are correct
            if (TextureWidth < 8 || TextureHeight < 8) return false;
            if ((TextureWidth & (TextureWidth - 1)) != 0 || (TextureHeight & (TextureHeight - 1)) != 0)
                return false;

            PixelCodec = GvrCodecList.GetPixelCodec((GvrPixelFormat)PixelFormat);
            DataCodec  = GvrCodecList.GetDataCodec((GvrDataFormat)DataFormat);

            if (DataCodec == null)      return false;
            if (!DataCodec.CanEncode) return false;
            if (PixelCodec == null && DataCodec.ClutEntries != 0) return false;
            if (!PixelCodec.CanEncode && DataCodec.ClutEntries != 0) return false;

            DataCodec.PixelCodec = PixelCodec;

            GbixOffset = 0x00;
            PvrtOffset = 0x10;

            // See if we need to palettize the bitmap and raw image data
            if (DataCodec.ClutEntries != 0)
                PalettizeBitmap();

            return true;
        }
        */
        /*
        // Write the Gbix header
        protected override byte[] WriteGbixHeader()
        {
            MemoryStream GbixHeader = new MemoryStream();
            using (BinaryWriter Writer = new BinaryWriter(GbixHeader))
            {
                Writer.Write(Encoding.UTF8.GetBytes("GCIX"));
                Writer.Write(0x00000008);
                Writer.Write(SwapUInt(GlobalIndex));
                Writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                Writer.Flush();
            }

            return GbixHeader.ToArray();
        }

        // Write the Pvrt header
        protected override byte[] WritePvrtHeader(int TextureSize)
        {
            // Before we write, set the clut data flag if the texture contains an internal clut
            if (DataCodec.ClutEntries != 0 && (DataFlags & GvrDataFlags.ExternalClut) == 0)
                DataFlags |= GvrDataFlags.InternalClut;

            MemoryStream PvrtHeader = new MemoryStream();
            using (BinaryWriter Writer = new BinaryWriter(PvrtHeader))
            {
                Writer.Write(Encoding.UTF8.GetBytes("GVRT"));
                Writer.Write((DataOffset + TextureSize) - 24);
                Writer.Write(new byte[] { 0x00, 0x00 });
                Writer.Write((byte)(((byte)PixelFormat << 4) | ((byte)DataFlags & 0x0F)));
                Writer.Write((byte)DataFormat);
                Writer.Write(SwapUShort(TextureWidth));
                Writer.Write(SwapUShort(TextureHeight));
                Writer.Flush();
            }

            return PvrtHeader.ToArray();
        }*/

        #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(string file, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat) : base(file)
        {
            if (RawImageData != null)
            {
                InitSuccess = Initalize(pixelFormat, dataFormat);
            }
            else
            {
                InitSuccess = false;
            }
        }

        /// <summary>
        /// Opens a texture to encode from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(byte[] source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source)
        {
            if (RawImageData != null)
            {
                InitSuccess = Initalize(pixelFormat, dataFormat);
            }
            else
            {
                InitSuccess = false;
            }
        }

        /// <summary>
        /// Opens a texture to encode from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(byte[] source, int offset, int length, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source, offset, length)
        {
            if (RawImageData != null)
            {
                InitSuccess = Initalize(pixelFormat, dataFormat);
            }
            else
            {
                InitSuccess = false;
            }
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(Stream source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source)
        {
            if (RawImageData != null)
            {
                InitSuccess = Initalize(pixelFormat, dataFormat);
            }
            else
            {
                InitSuccess = false;
            }
        }

        /// <summary>
        /// Opens a texture to encode from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(Stream source, int length, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source, length)
        {
            if (RawImageData != null)
            {
                InitSuccess = Initalize(pixelFormat, dataFormat);
            }
            else
            {
                InitSuccess = false;
            }
        }

        /// <summary>
        /// Opens a texture to encode from a bitmap.
        /// </summary>
        /// <param name="source">Bitmap to encode.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to. If the data format does not require a pixel format, use GvrPixelFormat.Unknown.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public GvrTextureEncoder(Bitmap source, GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
            : base(source)
        {
            if (RawImageData != null)
            {
                InitSuccess = Initalize(pixelFormat, dataFormat);
            }
            else
            {
                InitSuccess = false;
            }
        }

        private bool Initalize(GvrPixelFormat pixelFormat, GvrDataFormat dataFormat)
        {
            // Set the default values
            IncludeGbixHeader = true;
            GbixType = GvrGbixType.Gbix;
            GlobalIndex = 0;

            // Make sure the dimensions of the texture are valid
            if (TextureWidth < 4 || TextureHeight < 4 || TextureWidth > 1024 || TextureHeight > 1024)
                return false;

            if ((TextureWidth & (TextureWidth - 1)) != 0 || (TextureHeight & (TextureHeight - 1)) != 0)
                return false;

            // Set the data format and pixel format and load the appropiate codecs
            DataFormat = dataFormat;
            DataCodec = GvrDataCodec.GetDataCodec(DataFormat);

            // Make sure the data codec exists and we can encode to it
            if (DataCodec == null || !DataCodec.CanEncode) return false;

            // Only palettized formats require a pixel codec.
            if (DataCodec.PaletteEntries != 0)
            {
                PixelFormat = pixelFormat;
                PixelCodec = GvrPixelCodec.GetPixelCodec(PixelFormat);

                // Make sure the pixel codec exists and we can encode to it
                if (PixelCodec == null || !PixelCodec.CanEncode) return false;

                DataCodec.PixelCodec = PixelCodec;

                DataFlags = GvrDataFlags.InternalPalette;

                // Palettize the bitmap
                PalettizeBitmap();
            }
            else
            {
                PixelFormat = GvrPixelFormat.Unknown;
                PixelCodec = null;

                DataFlags = GvrDataFlags.None;
            }

            return true;
        }
        #endregion

        #region Texture Properties
        /// <summary>
        /// Indicates the magic code used for the GBIX header. This only matters if IncludeGbixHeader is true. The default value is GvrGbixType.Gbix.
        /// </summary>
        public GvrGbixType GbixType;

        /// <summary>
        /// The texture's data flags. The default value is GvrDataDlags.InternalClut if the data format is GvrDataFormat.Index4 or GvrDataFormat.Index8; GvrDataFlags.None otherwise.
        /// If both GvrDataFlags.InternalClut and GvrDataFlags.ExternalClut are set, GvrDataFlags.ExternalClut will take precedence.
        /// </summary>
        public GvrDataFlags DataFlags;

        //private GvrPixelFormat PixelFormat;
        //private GvrDataFormat DataFormat;
        public GvrPixelFormat PixelFormat { get; private set; }
        public GvrDataFormat DataFormat { get; private set; }
        #endregion

        #region Encode Texture
        protected override MemoryStream EncodeTexture()
        {
            // Before we write anything, let's make sure the data flags are set properly
            if ((DataFlags & GvrDataFlags.InternalPalette) != 0 && (DataFlags & GvrDataFlags.ExternalPalette) != 0)
            {
                // If both InternalClut and ExternalClut is set, default to ExternalClut.
                DataFlags &= ~GvrDataFlags.InternalPalette;
            }

            if ((DataFlags & GvrDataFlags.Palette) != 0 && DataCodec.PaletteEntries == 0)
            {
                // If this texture has no clut, then don't set any clut flags.
                DataFlags &= ~GvrDataFlags.Palette;
            }

            // Calculate what the length of the texture will be
            int textureLength = 16 + (TextureWidth * TextureHeight * DataCodec.Bpp / 8);
            if (IncludeGbixHeader)
            {
                textureLength += 16;
            }
            if ((DataFlags & GvrDataFlags.InternalPalette) != 0)
            {
                textureLength += (DataCodec.PaletteEntries * PixelCodec.Bpp / 8);
            }

            MemoryStream destination = new MemoryStream(textureLength);

            // Write out the GBIX header (if we are including one)
            if (IncludeGbixHeader)
            {
                if (GbixType == GvrGbixType.Gbix)
                {
                    destination.WriteByte((byte)'G');
                    destination.WriteByte((byte)'B');
                    destination.WriteByte((byte)'I');
                    destination.WriteByte((byte)'X');
                }
                else
                {
                    destination.WriteByte((byte)'G');
                    destination.WriteByte((byte)'C');
                    destination.WriteByte((byte)'I');
                    destination.WriteByte((byte)'X');
                }

                PTStream.WriteUInt32(destination, 8);
                PTStream.WriteUInt32BE(destination, GlobalIndex);
                PTStream.WriteUInt32(destination, 0);
            }

            // Write out the GVRT header
            destination.WriteByte((byte)'G');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'R');
            destination.WriteByte((byte)'T');

            PTStream.WriteInt32(destination, textureLength - 24);

            PTStream.WriteUInt16(destination, 0);
            if (PixelFormat != GvrPixelFormat.Unknown)
            {
                destination.WriteByte((byte)(((byte)PixelFormat << 4) | ((byte)DataFlags & 0x0F)));
            }
            else
            {
                destination.WriteByte((byte)((byte)DataFlags & 0xF));
            }
            destination.WriteByte((byte)DataFormat);

            PTStream.WriteUInt16BE(destination, TextureWidth);
            PTStream.WriteUInt16BE(destination, TextureHeight);

            // If we have an internal clut, write it
            if ((DataFlags & GvrDataFlags.InternalPalette) != 0)
            {
                byte[] clut = PixelCodec.EncodePalette(TextureClut, DataCodec.PaletteEntries);
                destination.Write(clut, 0, clut.Length);
            }

            // Write the texture data
            byte[] textureData = DataCodec.Encode(RawImageData, TextureWidth, TextureHeight, null);
            destination.Write(textureData, 0, textureData.Length);

            return destination;
        }
        #endregion
    }
}