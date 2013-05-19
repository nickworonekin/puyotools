using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace VrSharp.PvrTexture
{
    public class PvrTextureEncoder : VrTextureEncoder
    {
        #region Fields
        //public PvrPixelFormat PixelFormat { get; private set; }
        //public PvrDataFormat DataFormat { get; private set; }
        //public PvrCompressionFormat CompressionFormat { get; private set; } // Compression Format
        PvrCompressionCodec CompressionCodec;   // Compression Codec
        #endregion

        #region Constructors
        /*
        /// <summary>
        /// Open a bitmap from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the bitmap data.</param>
        /// <param name="PixelFormat">Pixel Format</param>
        /// <param name="DataFormat">Data Format</param>
        public PvrTextureEncoder(string file, PvrPixelFormat PixelFormat, PvrDataFormat DataFormat)
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
        public PvrTextureEncoder(Stream stream, PvrPixelFormat PixelFormat, PvrDataFormat DataFormat)
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
        public PvrTextureEncoder(byte[] array, PvrPixelFormat PixelFormat, PvrDataFormat DataFormat)
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
        public PvrTextureEncoder(Bitmap bitmap, PvrPixelFormat PixelFormat, PvrDataFormat DataFormat)
            : base(bitmap)
        {
            this.PixelFormat = PixelFormat;
            this.DataFormat  = DataFormat;

            InitSuccess = Initalize();
        }
        #endregion

        #region Misc
        /*
        /// <summary>
        /// Returns information about the texture.  (Use an explicit cast to get PvrTextureInfo.)
        /// </summary>
        /// <returns></returns>
        public override VrTextureInfo GetTextureInfo()
        {
            if (!InitSuccess) return new PvrTextureInfo();

            PvrTextureInfo TextureInfo    = new PvrTextureInfo();
            TextureInfo.TextureWidth      = TextureWidth;
            TextureInfo.TextureHeight     = TextureHeight;
            TextureInfo.PixelFormat       = PixelFormat;
            TextureInfo.DataFormat        = DataFormat;
            TextureInfo.CompressionFormat = CompressionFormat;

            return TextureInfo;
        }
         * */

        /*
        protected override byte[] DoPostEncodeEvents(byte[] TextureData)
        {
            if (CompressionFormat != PvrCompressionFormat.None)
                return CompressionCodec.Compress(TextureData, DataOffset, PixelCodec, DataCodec);

            return TextureData;
        }*/
        #endregion

        #region Clut
        protected override void CreateVpClut(byte[] ClutData, ushort NumClutEntries)
        {
            ClutEncoder = new PvpClutEncoder(ClutData, NumClutEntries, (PvrPixelFormat)PixelFormat);
        }
        #endregion

        /// <summary>
        /// Set the compression format.
        /// </summary>
        /// <param name="CompressionFormat">Compression Format</param>
        public void SetCompressionFormat(PvrCompressionFormat CompressionFormat)
        {
            if (!InitSuccess) return;

            if (CompressionFormat == PvrCompressionFormat.Rle && DataCodec.Bpp >= 8)
            {
                // We want to use Rle compression and our texture has a bpp of at least 8.
                CompressionCodec = PvrCompressionCodec.GetCompressionCodec(CompressionFormat);
                if (CompressionCodec != null)
                    this.CompressionFormat = PvrCompressionFormat.Rle;
                else
                    return; // Can't compress!
            }
        }

        /*
        // Initalize the bitmap
        private bool Initalize()
        {
            // Make sure the width and height are correct
            if (TextureWidth < 8 || TextureHeight < 8) return false;
            if ((TextureWidth & (TextureWidth - 1)) != 0 || (TextureHeight & (TextureHeight - 1)) != 0)
                return false;

            PixelCodec = PvrPixelCodec.GetPixelCodec((PvrPixelFormat)PixelFormat);
            DataCodec  = PvrDataCodec.GetDataCodec((PvrDataFormat)DataFormat);

            CompressionFormat = PvrCompressionFormat.None;
            CompressionCodec  = null;

            if (PixelCodec == null || DataCodec == null)           return false;
            if (!PixelCodec.CanEncode || !DataCodec.CanEncode) return false;
            if (!CanEncode((PvrPixelFormat)PixelFormat, (PvrDataFormat)DataFormat, TextureWidth, TextureHeight)) return false;

            DataCodec.PixelCodec = PixelCodec;

            GbixOffset = 0x00;
            PvrtOffset = 0x10;

            // See if we need to palettize the bitmap and raw image data
            if (DataCodec.ClutEntries != 0)
                PalettizeBitmap();

            return true;
        }

        // Checks to see if we can encode the texture based on data format specific things
        private bool CanEncode(PvrPixelFormat PixelFormat, PvrDataFormat DataFormat, int width, int height)
        {
            // The converter should check to see that a pixel codec and data codec exists,
            // along with checking that width >= 8 and height >= 8, and width and height are powers of 2.
            switch (DataFormat)
            {
                case PvrDataFormat.SquareTwiddled:
                case PvrDataFormat.SquareTwiddledMipmaps:
                case PvrDataFormat.Vq:
                case PvrDataFormat.VqMipmaps:
                case PvrDataFormat.SquareTwiddledMipmapsDup:
                    return (width == height);
                case PvrDataFormat.SmallVq:
                    return (width == height && width <= 64);
                case PvrDataFormat.SmallVqMipmaps:
                    return (width == height && width <= 32);
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
                Writer.Write(new byte[] { 0x20, 0x20, 0x20, 0x20 });
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
        /// <summary>
        /// The texture's compression format. The default value is PvrCompressionFormat.None.
        /// </summary>
        public PvrCompressionFormat CompressionFormat;

        public PvrPixelFormat PixelFormat { get; private set; }
        public PvrDataFormat DataFormat { get; private set; }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Opens a texture to encode from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        /// <param name="pixelFormat">Pixel format to encode the texture to.</param>
        /// <param name="dataFormat">Data format to encode the texture to.</param>
        public PvrTextureEncoder(string file, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat) : base(file)
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
        public PvrTextureEncoder(byte[] source, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
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
        public PvrTextureEncoder(byte[] source, int offset, int length, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
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
        public PvrTextureEncoder(Stream source, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
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
        public PvrTextureEncoder(Stream source, int length, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
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
        public PvrTextureEncoder(Bitmap source, PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
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

        private bool Initalize(PvrPixelFormat pixelFormat, PvrDataFormat dataFormat)
        {
            // Set the default values
            IncludeGbixHeader = true;
            GlobalIndex = 0;
            CompressionFormat = PvrCompressionFormat.None;

            // Make sure the dimensions of the texture are valid
            if (TextureWidth < 4 || TextureHeight < 4 || TextureWidth > 1024 || TextureHeight > 1024)
                return false;

            if ((TextureWidth & (TextureWidth - 1)) != 0 || (TextureHeight & (TextureHeight - 1)) != 0)
                return false;

            // Set the data format and pixel format and load the appropiate codecs
            PixelFormat = pixelFormat;
            PixelCodec = PvrPixelCodec.GetPixelCodec(PixelFormat);

            DataFormat = dataFormat;
            DataCodec = PvrDataCodec.GetDataCodec(DataFormat);

            // Make sure the pixel and data codecs exists and we can encode to it
            if (PixelCodec == null || !PixelCodec.CanEncode) return false;
            if (DataCodec == null || !DataCodec.CanEncode) return false;

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

            // Compress the texture
            if (CompressionFormat != PvrCompressionFormat.None)
            {
                CompressionCodec = PvrCompressionCodec.GetCompressionCodec(CompressionFormat);

                if (CompressionCodec != null)
                {
                    // Ok, we need to convert the current stream to an array, compress it, then write it back to a new stream
                    byte[] buffer = destination.ToArray();
                    buffer = CompressionCodec.Compress(buffer, (IncludeGbixHeader ? 0x10 : 0), PixelCodec, DataCodec);

                    destination = new MemoryStream();
                    destination.Write(buffer, 0, buffer.Length);
                }
            }

            return destination;
        }
        #endregion
    }
}