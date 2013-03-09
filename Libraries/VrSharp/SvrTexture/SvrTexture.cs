using System;
using System.IO;
using System.Drawing;

namespace VrSharp.SvrTexture
{
    public class SvrTexture : VrTexture
    {
        #region Constructors
        /// <summary>
        /// Open a Svr texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public SvrTexture(string file)
            : base(file)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svr texture from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the texture data.</param>
        public SvrTexture(Stream stream)
            : base(stream)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svr texture from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public SvrTexture(Stream stream, int length)
            : base(stream, length)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svr texture from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the texture data.</param>
        public SvrTexture(byte[] array)
            : base(array)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svr texture from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public SvrTexture(byte[] array, long offset, int length)
            : base(array, offset, length)
        {
            InitSuccess = ReadHeader();
        }
        #endregion

        #region Clut
        /// <summary>
        /// Set the clut data from an external clut file.
        /// </summary>
        /// <param name="clut">A SvpClut object</param>
        public override void SetClut(VpClut clut)
        {
            if (!(clut is SvpClut)) // Make sure this is a SvpClut object
            {
                throw new ArgumentException(String.Format(
                    "VpClut type is {0} when it needs to be SvpClut.",
                    clut.GetType()));
            }

            base.SetClut(clut);
        }
        #endregion

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

        #region Header
        // Read the header and sets up the appropiate values.
        // Returns true if successful, otherwise false
        private bool ReadHeader()
        {
            // Make sure this is a Svr Texture
            if (!IsSvrTexture(TextureData))
                return false;

            // Get the header offsets
            if (Compare(TextureData, "GBIX", 0x00))
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
            TextureWidth  = BitConverter.ToUInt16(TextureData, PvrtOffset + 0x0C);
            TextureHeight = BitConverter.ToUInt16(TextureData, PvrtOffset + 0x0E);

            PixelFormat = TextureData[PvrtOffset + 0x08];
            DataFormat  = TextureData[PvrtOffset + 0x09];

            // Get the codecs and make sure we can decode using them
            PixelCodec = SvrCodecList.GetPixelCodec((SvrPixelFormat)PixelFormat);
            DataCodec  = SvrCodecList.GetDataCodec((SvrDataFormat)DataFormat);
            if (PixelCodec == null || !PixelCodec.CanDecode()) return false;
            if (DataCodec == null  || !DataCodec.CanDecode()) return false;

            // Set the clut and data offsets
            if (DataCodec.GetNumClutEntries() == 0 || DataCodec.NeedsExternalClut())
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

        // Checks if the input file is a svr
        public static bool IsSvrTexture(byte[] data, long offset, int length)
        {
            // Gbix and Pvrt
            if (length >= 0x20 &&
                Compare(data, "GBIX", (int)offset + 0x00) &&
                Compare(data, "PVRT", (int)offset + 0x10) &&
                data[offset + 0x19] >= 0x60 && data[offset + 0x19] < 0x70 &&
                BitConverter.ToUInt32(data, (int)offset + 0x14) == length - 24)
                return true;
            // Pvrt
            else if (length >= 0x10 &&
                Compare(data, "PVRT", (int)offset + 0x00) &&
                data[offset + 0x19] >= 0x60 && data[offset + 0x19] < 0x70 &&
                BitConverter.ToUInt32(data, (int)offset + 0x04) == length - 8)
                return true;

            return false;
        }

        public static bool IsSvrTexture(byte[] data)
        {
            return IsSvrTexture(data, 0, data.Length);
        }
        public static bool IsSvrTexture(Stream data, int length)
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

            return IsSvrTexture(buffer, 0, length);
        }
        #endregion
    }
}