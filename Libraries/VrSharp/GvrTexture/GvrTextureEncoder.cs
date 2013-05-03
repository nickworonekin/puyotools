using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace VrSharp.GvrTexture
{
    public class GvrTextureEncoder : VrTextureEncoder
    {
        #region Fields
        public GvrPixelFormat PixelFormat { get; private set; }
        public GvrDataFormat DataFormat { get; private set; }
        public GvrDataFlags DataFlags { get; private set; } // Data Flags
        #endregion

        #region Constructors
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
        #endregion

        #region Enums
        public enum GvrGbixType
        {
            Gamecube,
            Wii,
        }
        #endregion

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
            ClutEncoder = new GvpClutEncoder(ClutData, NumClutEntries, (GvrPixelFormat)PixelFormat);
        }
        #endregion

        /// <summary>
        /// Set data flags.
        /// </summary>
        /// <param name="Mipmaps">Texture contains mipmaps.</param>
        /// <param name="ExternalClut">Palettized texture contains an external clut.</param>
        public void SetDataFlags(bool Mipmaps, bool ExternalClut)
        {
            if (!InitSuccess) return;

            if (Mipmaps && DataCodec.GetNumClutEntries() == 0) // No mipmaps for palettized textures yet
                DataFlags |= GvrDataFlags.Mipmaps;
            if (ExternalClut && DataCodec.GetNumClutEntries() != 0)
                DataFlags |= GvrDataFlags.ExternalClut;
        }

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
            if (PixelCodec == null && DataCodec.GetNumClutEntries() != 0)      return false;
            if (!PixelCodec.CanEncode && DataCodec.GetNumClutEntries() != 0) return false;

            DataCodec.PixelCodec = PixelCodec;

            GbixOffset = 0x00;
            PvrtOffset = 0x10;

            // See if we need to palettize the bitmap and raw image data
            if (DataCodec.GetNumClutEntries() != 0)
                PalettizeBitmap();

            return true;
        }

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
            if (DataCodec.GetNumClutEntries() != 0 && (DataFlags & GvrDataFlags.ExternalClut) == 0)
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
        }
    }
}