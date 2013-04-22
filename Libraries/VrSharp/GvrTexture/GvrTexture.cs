using System;
using System.IO;
using System.Drawing;

namespace VrSharp.GvrTexture
{
    public class GvrTexture : VrTexture
    {
        #region Fields
        byte DataFlags; // Data Flags
        #endregion

        #region Constructors
        /// <summary>
        /// Open a Gvr texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public GvrTexture(string file)
            : base(file)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Gvr texture from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the texture data.</param>
        public GvrTexture(Stream stream)
            : base(stream)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Gvr texture from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GvrTexture(Stream stream, int length)
            : base(stream, length)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Gvr texture from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the texture data.</param>
        public GvrTexture(byte[] array)
            : base(array)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Gvr texture from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GvrTexture(byte[] array, long offset, int length)
            : base(array, offset, length)
        {
            InitSuccess = ReadHeader();
        }
        #endregion

        #region Clut
        /// <summary>
        /// Set the clut data from an external clut file.
        /// </summary>
        /// <param name="clut">A GvpClut object</param>
        public override void SetClut(VpClut clut)
        {
            if (!(clut is GvpClut)) // Make sure this is a GvpClut object
            {
                throw new ArgumentException(String.Format(
                    "VpClut type is {0} when it needs to be GvpClut.",
                    clut.GetType()));
            }

            base.SetClut(clut);
        }

        /// <summary>
        /// Returns if the texture needs an external clut file.
        /// </summary>
        /// <returns></returns>
        public override bool NeedsExternalClut()
        {
            if (!InitSuccess) return false;

            return ((DataFlags & 0x02) != 0);
        }
        #endregion

        #region Misc
        /// <summary>
        /// Returns information about the texture. (Use an explicit cast to get GvrTextureInfo.)
        /// </summary>
        /// <returns></returns>
        public override VrTextureInfo GetTextureInfo()
        {
            if (!InitSuccess) return new GvrTextureInfo();

            GvrTextureInfo TextureInfo = new GvrTextureInfo();
            TextureInfo.GlobalIndex    = GlobalIndex;
            TextureInfo.TextureWidth   = TextureWidth;
            TextureInfo.TextureHeight  = TextureHeight;
            TextureInfo.PixelFormat    = TextureInfo.DataFormat;
            TextureInfo.DataFormat     = DataFormat;
            TextureInfo.DataFlags      = DataFlags;
            TextureInfo.PvrtOffset     = PvrtOffset;

            return TextureInfo;
        }
        #endregion

        #region Header
        // Read the header and sets up the appropiate values.
        // Returns true if successful, otherwise false
        private bool ReadHeader()
        {
            // Make sure this is a Gvr Texture
            if (!IsGvrTexture(TextureData))
                return false;

            // Get the header offsets
            if (Compare(TextureData, "GBIX", 0x00) ||
                Compare(TextureData, "GCIX", 0x00))
            {
                GbixOffset = 0x00;
                PvrtOffset = 0x10;
            }
            else
            {
                GbixOffset = -1;
                PvrtOffset = 0x00;
            }

            // Read the file information
            if (GbixOffset != -1)
            {
                GlobalIndex = (uint)(TextureData[GbixOffset + 0x08] << 24 | TextureData[GbixOffset + 0x09] << 16 | TextureData[GbixOffset + 0x0A] << 8 | TextureData[GbixOffset + 0x0B]);
            }
            else
            {
                GlobalIndex = 0;
            }

            TextureWidth  = (ushort)((TextureData[PvrtOffset + 0x0C] << 8) | TextureData[PvrtOffset + 0x0D]);
            TextureHeight = (ushort)((TextureData[PvrtOffset + 0x0E] << 8) | TextureData[PvrtOffset + 0x0F]);

            PixelFormat = (byte)(TextureData[PvrtOffset + 0x0A] >> 4); // Only the first 4 bits matter
            DataFormat  = TextureData[PvrtOffset + 0x0B];
            DataFlags   = (byte)(TextureData[PvrtOffset + 0x0A] & 0x0F); // Only the last 4 bits matter

            // Get the codecs and make sure we can decode using them
            PixelCodec = GvrCodecList.GetPixelCodec((GvrPixelFormat)PixelFormat);
            DataCodec  = GvrCodecList.GetDataCodec((GvrDataFormat)DataFormat);
            if (DataCodec == null || !DataCodec.CanDecode()) return false;
            if ((DataFlags & 0x0A) != 0 && (PixelCodec == null || !PixelCodec.CanDecode())) return false;

            // Set the clut and data offsets
            if ((DataFlags & 0x08) == 0 || DataCodec.GetNumClutEntries() == 0 || NeedsExternalClut())
            {
                ClutOffset = -1;
                DataOffset = PvrtOffset + 0x10;
            }
            else
            {
                ClutOffset = PvrtOffset + 0x10;
                DataOffset = ClutOffset + (DataCodec.GetNumClutEntries() * (PixelCodec.GetBpp() / 8));
            }

            RawImageData = new byte[TextureWidth * TextureHeight * 4];
            return true;
        }

        // Checks if the input file is a gvr
        public static bool IsGvrTexture(byte[] data, long offset, int length)
        {
            // Gbix and Gvrt
            if (length >= 0x20 &&
                Compare(data, "GBIX", (int)offset + 0x00) &&
                Compare(data, "GVRT", (int)offset + 0x10) &&
                BitConverter.ToUInt32(data, (int)offset + 0x14) == length - 24)
                return true;
            // Gcix and Gvrt
            else if (length >= 0x20 &&
                Compare(data, "GCIX", (int)offset + 0x00) &&
                Compare(data, "GVRT", (int)offset + 0x10) &&
                BitConverter.ToUInt32(data, (int)offset + 0x14) == length - 24)
                return true;
            // Gvrt
            else if (length >= 0x10 &&
                Compare(data, "GVRT", (int)offset + 0x00) &&
                BitConverter.ToUInt32(data, (int)offset + 0x04) == length - 8)
                return true;

            return false;
        }

        public static bool IsGvrTexture(byte[] data)
        {
            return IsGvrTexture(data, 0, data.Length);
        }
        public static bool IsGvrTexture(Stream data, int length)
        {
            // If it's less than 16 bytes, then it is not a texture
            if (length < 0x10)
                return false;

            long oldPosition = data.Position;
            byte[] buffer = new byte[0x20];

            if (length > 0x20)
                data.Read(buffer, 0, 0x20);
            else
                data.Read(buffer, 0, 0x10);

            data.Position = oldPosition;

            return IsGvrTexture(buffer, 0, length);
        }
        #endregion
    }
}