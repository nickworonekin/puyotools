using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace VrSharp.SvrTexture
{
    public class SvrTextureEncoder : VrTextureEncoder
    {
        #region Fields
        //public SvrPixelFormat PixelFormat { get; private set; }
        //public SvrDataFormat DataFormat { get; private set; }
        #endregion

        /*
        #region Constructors
        /// <summary>
        /// Open a bitmap from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the bitmap data.</param>
        /// <param name="PixelFormat">Pixel Format</param>
        /// <param name="DataFormat">Data Format</param>
        public SvrTextureEncoder(string file, SvrPixelFormat PixelFormat, SvrDataFormat DataFormat)
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
        public SvrTextureEncoder(Stream stream, SvrPixelFormat PixelFormat, SvrDataFormat DataFormat)
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
        public SvrTextureEncoder(byte[] array, SvrPixelFormat PixelFormat, SvrDataFormat DataFormat)
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
        public SvrTextureEncoder(Bitmap bitmap, SvrPixelFormat PixelFormat, SvrDataFormat DataFormat)
            : base(bitmap)
        {
            this.PixelFormat = PixelFormat;
            this.DataFormat  = DataFormat;

            InitSuccess = Initalize();
        }
        #endregion

        /*
        #region Misc
        /// <summary>
        /// Returns information about the texture.  (Use an explicit cast to get SvrTextureInfo.)
        /// </summary>
        /// <returns></returns>
        public override VrTextureInfo GetTextureInfo()
        {
            if (!InitSuccess) return new SvrTextureInfo();

            SvrTextureInfo TextureInfo = new SvrTextureInfo();
            TextureInfo.TextureWidth   = TextureWidth;
            TextureInfo.TextureHeight  = TextureHeight;
            TextureInfo.PixelFormat    = PixelFormat;
            TextureInfo.DataFormat     = DataFormat;

            return TextureInfo;
        }
        #endregion
         * */

        #region Clut
        protected override void CreateVpClut(byte[] ClutData, ushort NumClutEntries)
        {
            ClutEncoder = new SvpPaletteEncoder(ClutData, NumClutEntries, (SvrPixelFormat)PixelFormat);
        }
        #endregion

        /*
        // Initalize the bitmap
        private bool Initalize()
        {
            // Make sure the width and height are correct
            if (TextureWidth < 8 || TextureHeight < 8) return false;
            if ((TextureWidth & (TextureWidth - 1)) != 0 || (TextureHeight & (TextureHeight - 1)) != 0)
                return false;

            PixelCodec = SvrPixelCodec.GetPixelCodec((SvrPixelFormat)PixelFormat);
            DataCodec  = SvrDataCodec.GetDataCodec((SvrDataFormat)DataFormat);

            if (PixelCodec == null || DataCodec == null)           return false;
            if (!PixelCodec.CanEncode || !DataCodec.CanEncode) return false;
            if (!CanEncode((SvrPixelFormat)PixelFormat, (SvrDataFormat)DataFormat, TextureWidth, TextureHeight)) return false;

            DataCodec.PixelCodec = PixelCodec;

            GbixOffset = 0x00;
            PvrtOffset = 0x10;

            // See if we need to palettize the bitmap and raw image data
            if (DataCodec.ClutEntries != 0)
                PalettizeBitmap();

            return true;
        }

        // Checks to see if we can encode the texture based on data format specific things
        private bool CanEncode(SvrPixelFormat PixelFormat, SvrDataFormat DataFormat, int width, int height)
        {
            // The converter should check to see that a pixel codec and data codec exists,
            // along with checking that width >= 8 and height >= 8.
            switch (DataFormat)
            {
                case SvrDataFormat.Index4RectRgb5a3:
                case SvrDataFormat.Index8RectRgb5a3:
                    return (PixelFormat == SvrPixelFormat.Rgb5a3);
                case SvrDataFormat.Index4SqrRgb5a3:
                case SvrDataFormat.Index8SqrRgb5a3:
                    return (width == height && PixelFormat == SvrPixelFormat.Rgb5a3);
                case SvrDataFormat.Index4RectArgb8:
                case SvrDataFormat.Index8RectArgb8:
                    return (PixelFormat == SvrPixelFormat.Argb8888);
                case SvrDataFormat.Index4SqrArgb8:
                case SvrDataFormat.Index8SqrArgb8:
                    return (width == height && PixelFormat == SvrPixelFormat.Argb8888);
            }

            return true;
        }*/

        /*
        // Write the Gbix header
        protected override byte[] WriteGbixHeader()
        {
            MemoryStream GbixHeader = new MemoryStream();
            using (BinaryWriter Writer = new BinaryWriter(GbixHeader))
            {
                Writer.Write(Encoding.UTF8.GetBytes("GBIX"));
                Writer.Write(0x00000008);
                Writer.Write(GlobalIndex);
                Writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                Writer.Flush();
            }

            return GbixHeader.ToArray();
        }

        // Write the Pvrt header
        protected override byte[] WritePvrtHeader(int TextureSize)
        {
            MemoryStream PvrtHeader = new MemoryStream();
            using (BinaryWriter Writer = new BinaryWriter(PvrtHeader))
            {
                Writer.Write(Encoding.UTF8.GetBytes("PVRT"));
                Writer.Write((DataOffset + TextureSize) - 24);
                Writer.Write((byte)PixelFormat);
                Writer.Write((byte)DataFormat);
                Writer.Write(new byte[] { 0x00, 0x00 });
                Writer.Write(TextureWidth);
                Writer.Write(TextureHeight);
                Writer.Flush();
            }

            return PvrtHeader.ToArray();
        }*/

        #region Texture Properties
        public SvrPixelFormat PixelFormat { get; private set; }
        public SvrDataFormat DataFormat { get; private set; }
        #endregion

                #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public SvrTextureEncoder(string file, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat) : base(file)
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
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public SvrTextureEncoder(byte[] source, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
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
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public SvrTextureEncoder(byte[] source, int offset, int length, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
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
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public SvrTextureEncoder(Stream source, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
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
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public SvrTextureEncoder(Stream source, int length, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
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
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public SvrTextureEncoder(Bitmap source, SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
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

        private bool Initalize(SvrPixelFormat pixelFormat, SvrDataFormat dataFormat)
        {
            // Set the default values
            IncludeGbixHeader = true;
            GlobalIndex = 0;

            // Make sure the dimensions of the texture are valid
            if (TextureWidth < 4 || TextureHeight < 4 || TextureWidth > 1024 || TextureHeight > 1024)
                return false;

            if ((TextureWidth & (TextureWidth - 1)) != 0 || (TextureHeight & (TextureHeight - 1)) != 0)
                return false;

            // Set the data format and pixel format and load the appropiate codecs
            PixelFormat = pixelFormat;
            PixelCodec = SvrPixelCodec.GetPixelCodec(PixelFormat);

            DataFormat = dataFormat;
            DataCodec = SvrDataCodec.GetDataCodec(DataFormat);

            // Make sure the pixel and data codecs exists and we can encode to it
            if (PixelCodec == null || !PixelCodec.CanEncode) return false;
            if (DataCodec == null || !DataCodec.CanEncode) return false;

            // Set the correct data format (it's ok to do it after getting the codecs).
            if (DataFormat == SvrDataFormat.Index4RectRgb5a3 || DataFormat == SvrDataFormat.Index4SqrRgb5a3 ||
                DataFormat == SvrDataFormat.Index4RectArgb8 || DataFormat == SvrDataFormat.Index4SqrArgb8)
            {
                if (TextureWidth == TextureHeight) // Square texture
                {
                    if (PixelFormat == SvrPixelFormat.Rgb5a3)
                    {
                        DataFormat = SvrDataFormat.Index4SqrRgb5a3;
                    }
                    else
                    {
                        DataFormat = SvrDataFormat.Index4SqrArgb8;
                    }
                }
                else // Rectangular texture
                {
                    if (PixelFormat == SvrPixelFormat.Rgb5a3)
                    {
                        DataFormat = SvrDataFormat.Index4RectRgb5a3;
                    }
                    else
                    {
                        DataFormat = SvrDataFormat.Index4RectArgb8;
                    }
                }
            }

            else if (DataFormat == SvrDataFormat.Index8RectRgb5a3 || DataFormat == SvrDataFormat.Index8SqrRgb5a3 ||
                DataFormat == SvrDataFormat.Index8RectArgb8 || DataFormat == SvrDataFormat.Index8SqrArgb8)
            {
                if (TextureWidth == TextureHeight) // Square texture
                {
                    if (PixelFormat == SvrPixelFormat.Rgb5a3)
                    {
                        DataFormat = SvrDataFormat.Index8SqrRgb5a3;
                    }
                    else
                    {
                        DataFormat = SvrDataFormat.Index8SqrArgb8;
                    }
                }
                else // Rectangular texture
                {
                    if (PixelFormat == SvrPixelFormat.Rgb5a3)
                    {
                        DataFormat = SvrDataFormat.Index8RectRgb5a3;
                    }
                    else
                    {
                        DataFormat = SvrDataFormat.Index8RectArgb8;
                    }
                }
            }

            // Palettize the bitmap if this data format is palettized.
            if (DataCodec.ClutEntries != 0)
            {
                PalettizeBitmap();
            }

            return true;
        }
        #endregion

        #region Encode Texture
        protected override MemoryStream EncodeTexture()
        {
            // Calculate what the length of the texture will be
            int textureLength = 16 + (TextureWidth * TextureHeight * DataCodec.Bpp / 8);
            if (IncludeGbixHeader)
            {
                textureLength += 16;
            }
            if (DataCodec.ClutEntries != 0 && !DataCodec.NeedsExternalClut)
            {
                textureLength += (DataCodec.ClutEntries * PixelCodec.Bpp / 8);
            }

            MemoryStream destination = new MemoryStream(textureLength);

            // Write out the GBIX header (if we are including one)
            if (IncludeGbixHeader)
            {
                destination.WriteByte((byte)'G');
                destination.WriteByte((byte)'B');
                destination.WriteByte((byte)'I');
                destination.WriteByte((byte)'X');

                PTStream.WriteUInt32(destination, 8);
                PTStream.WriteUInt32(destination, GlobalIndex);
                PTStream.WriteUInt32(destination, 0);
            }

            // Write out the PVRT header
            destination.WriteByte((byte)'P');
            destination.WriteByte((byte)'V');
            destination.WriteByte((byte)'R');
            destination.WriteByte((byte)'T');

            PTStream.WriteInt32(destination, textureLength - 24);

            destination.WriteByte((byte)PixelFormat);
            destination.WriteByte((byte)DataFormat);
            PTStream.WriteUInt16(destination, 0);

            PTStream.WriteUInt16(destination, TextureWidth);
            PTStream.WriteUInt16(destination, TextureHeight);

            // If we have an internal clut, write it
            if (DataCodec.ClutEntries != 0 && !DataCodec.NeedsExternalClut)
            {
                byte[] clut = PixelCodec.EncodeClut(TextureClut, DataCodec.ClutEntries);
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