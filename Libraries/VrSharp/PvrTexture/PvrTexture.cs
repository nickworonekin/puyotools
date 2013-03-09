using System;
using System.IO;
using System.Drawing;

namespace VrSharp.PvrTexture
{
    public class PvrTexture : VrTexture
    {
        #region Fields
        PvrCompressionFormat CompressionFormat; // Compression Format
        PvrCompressionCodec CompressionCodec;   // Compression Codec
        #endregion

        #region Constructors
        /// <summary>
        /// Open a Pvr texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public PvrTexture(string file)
            : base(file)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Pvr texture from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the texture data.</param>
        public PvrTexture(Stream stream)
            : base(stream)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Pvr texture from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public PvrTexture(Stream stream, int length)
            : base(stream, length)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Pvr texture from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the texture data.</param>
        public PvrTexture(byte[] array)
            : base(array)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Pvr texture from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public PvrTexture(byte[] array, long offset, int length)
            : base(array, offset, length)
        {
            InitSuccess = ReadHeader();
        }
        #endregion

        #region Clut
        /// <summary>
        /// Set the clut data from an external clut file.
        /// </summary>
        /// <param name="clut">A PvpClut object</param>
        public override void SetClut(VpClut clut)
        {
            if (!(clut is PvpClut)) // Make sure this is a PvpClut object
            {
                throw new ArgumentException(String.Format(
                    "VpClut type is {0} when it needs to be PvpClut.",
                    clut.GetType()));
            }

            base.SetClut(clut);
        }
        #endregion

        #region Misc
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
        #endregion

        #region Header
        // Read the header and sets up the appropiate values.
        // Returns true if successful, otherwise false
        private bool ReadHeader()
        {
            // Make sure this is a Pvr Texture
            if (!IsPvrTexture(TextureData))
                return false;

            // Get the header offsets
            if (Compare(TextureData, "GBIX", 0x00))
            {
                GbixOffset = 0x00;
                PvrtOffset = 0x10;
            }
            else if (Compare(TextureData, "GBIX", 0x04))
            {
                GbixOffset = 0x04;
                PvrtOffset = 0x14;
            }
            else if (Compare(TextureData, "PVRT", 0x04))
            {
                GbixOffset = -1;
                PvrtOffset = 0x04;
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
            PixelCodec = PvrCodecList.GetPixelCodec((PvrPixelFormat)PixelFormat);
            DataCodec  = PvrCodecList.GetDataCodec((PvrDataFormat)DataFormat);
            if (PixelCodec == null || !PixelCodec.CanDecode()) return false;
            if (DataCodec == null  || !DataCodec.CanDecode())  return false;

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

            // Get the compression format & decompress the pvr
            CompressionFormat = GetCompressionFormat(TextureData, PvrtOffset, DataOffset);
            CompressionCodec  = PvrCodecList.GetCompressionCodec(CompressionFormat);

            if (CompressionFormat != PvrCompressionFormat.None && CompressionCodec != null)
            {
                TextureData = CompressionCodec.Decompress(TextureData, DataOffset, PixelCodec, DataCodec);

                // Now place the offsets in the appropiate area
                if (CompressionFormat == PvrCompressionFormat.Rle)
                {
                    if (GbixOffset != -1) GbixOffset -= 4;
                    PvrtOffset -= 4;
                    if (ClutOffset != -1) ClutOffset -= 4;
                    DataOffset -= 4;
                }
            }

            RawImageData = new byte[TextureWidth * TextureHeight * 4];
            return true;
        }

        // Checks if the input file is a pvr
        public static bool IsPvrTexture(byte[] data, long offset, int length)
        {
            // Gbix and Pvrt
            if (length >= 0x20 &&
                Compare(data, "GBIX", (int)offset + 0x00) &&
                Compare(data, "PVRT", (int)offset + 0x10) &&
                data[offset + 0x19] < 0x60 &&
                BitConverter.ToUInt32(data, (int)offset + 0x14) == length - 24)
                return true;
            // Pvrt
            else if (length >= 0x10 &&
                Compare(data, "PVRT", (int)offset + 0x00) &&
                data[offset + 0x09] < 0x60 &&
                BitConverter.ToUInt32(data, (int)offset + 0x04) == length - 8)
                return true;
            // Gbix and Pvrt (w/ Rle Compression)
            else if (length >= 0x24 &&
                Compare(data, "GBIX", (int)offset + 0x04) &&
                Compare(data, "PVRT", (int)offset + 0x14) &&
                data[offset + 0x1D] < 0x60 &&
                BitConverter.ToUInt32(data, (int)offset + 0x18) == BitConverter.ToUInt32(data, (int)offset + 0x00) - 24)
                return true;
            // Pvrt (w/ Rle Compression)
            else if (length >= 0x14 &&
                Compare(data, "PVRT", (int)offset + 0x04) &&
                data[offset + 0x0D] < 0x60 &&
                BitConverter.ToUInt32(data, (int)offset + 0x08) == BitConverter.ToUInt32(data, (int)offset + 0x00) - 8)
                return true;

            return false;
        }

        public static bool IsPvrTexture(byte[] data)
        {
            return IsPvrTexture(data, 0, data.Length);
        }
        public static bool IsPvrTexture(Stream data, int length)
        {
            // If it's less than 16 bytes, then it is not a texture
            if (length < 0x10)
                return false;

            long oldPosition = data.Position;
            byte[] buffer = new byte[0x24];

            if (length > 0x24)
                data.Read(buffer, 0, 0x24);
            else if (length > 0x20)
                data.Read(buffer, 0, 0x20);
            else
                data.Read(buffer, 0, 0x10);

            data.Position = oldPosition;

            return IsPvrTexture(buffer, 0, length);
        }

        // Gets the compression format used on the pvr
        private PvrCompressionFormat GetCompressionFormat(byte[] data, int PvrtOffset, int DataOffset)
        {
            // Rle Compression
            if (BitConverter.ToUInt32(data, 0x00) == BitConverter.ToUInt32(data, PvrtOffset + 4) - PvrtOffset + DataOffset + 8)
                return PvrCompressionFormat.Rle;

            return PvrCompressionFormat.None;
        }
        #endregion
    }
}