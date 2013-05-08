using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace VrSharp.SvrTexture
{
    public class SvrTextureEncoder : VrTextureEncoder
    {
        #region Fields
        public SvrPixelFormat PixelFormat { get; private set; }
        public SvrDataFormat DataFormat { get; private set; }
        #endregion

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
            ClutEncoder = new SvpClutEncoder(ClutData, NumClutEntries, (SvrPixelFormat)PixelFormat);
        }
        #endregion

        // Initalize the bitmap
        private bool Initalize()
        {
            // Make sure the width and height are correct
            if (TextureWidth < 8 || TextureHeight < 8) return false;
            if ((TextureWidth & (TextureWidth - 1)) != 0 || (TextureHeight & (TextureHeight - 1)) != 0)
                return false;

            PixelCodec = SvrCodecList.GetPixelCodec((SvrPixelFormat)PixelFormat);
            DataCodec  = SvrCodecList.GetDataCodec((SvrDataFormat)DataFormat);

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
        }

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
        }

        #region Texture Properties
        //private SvrPixelFormat PixelFormat;
        //private SvrDataFormat DataFormat;
        #endregion
    }
}