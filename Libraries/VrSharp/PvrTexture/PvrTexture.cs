using System;
using System.IO;
using System.Drawing;
using System.Text;

namespace VrSharp.PvrTexture
{
    public class PvrTexture : VrTexture
    {
        #region Fields
        private PvrCompressionCodec CompressionCodec; // Compression Codec
        #endregion

        #region Texture Properties
        /// <summary>
        /// The texture's pixel format.
        /// </summary>
        public PvrPixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// The texture's data format.
        /// </summary>
        public PvrDataFormat DataFormat { get; private set; }

        /// <summary>
        /// The texture's compression format (if it is compressed).
        /// </summary>
        public PvrCompressionFormat CompressionFormat { get; private set; }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a PVR texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public PvrTexture(string file) : base(file) { }

        /// <summary>
        /// Open a PVR texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        public PvrTexture(byte[] source) : base(source) { }

        /// <summary>
        /// Open a PVR texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public PvrTexture(byte[] source, int offset, int length) : base(source, offset, length) { }

        /// <summary>
        /// Open a PVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public PvrTexture(Stream source) : base(source) { }

        /// <summary>
        /// Open a PVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public PvrTexture(Stream source, int length) : base(source, length) { }

        protected override bool Initalize()
        {
            // Check to see if what we are dealing with is a PVR texture
            if (!Is(TextureData))
                return false;

            // Determine the offsets of the GBIX (if present) and PVRT header chunks.
            if (PTMethods.Contains(TextureData, 0x00, Encoding.UTF8.GetBytes("GBIX")))
            {
                GbixOffset = 0x00;
                PvrtOffset = 0x10;
            }
            else if (PTMethods.Contains(TextureData, 0x04, Encoding.UTF8.GetBytes("GBIX")))
            {
                GbixOffset = 0x04;
                PvrtOffset = 0x14;
            }
            else if (PTMethods.Contains(TextureData, 0x04, Encoding.UTF8.GetBytes("PVRT")))
            {
                GbixOffset = -1;
                PvrtOffset = 0x04;
            }
            else
            {
                GbixOffset = -1;
                PvrtOffset = 0x00;
            }

            // Read the global index (if it is present). If it is not present, just set it to 0.
            if (GbixOffset != -1)
            {
                GlobalIndex = BitConverter.ToUInt32(TextureData, GbixOffset + 0x08);
            }
            else
            {
                GlobalIndex = 0;
            }

            // Read information about the texture
            TextureWidth  = BitConverter.ToUInt16(TextureData, PvrtOffset + 0x0C);
            TextureHeight = BitConverter.ToUInt16(TextureData, PvrtOffset + 0x0E);

            PixelFormat = (PvrPixelFormat)TextureData[PvrtOffset + 0x08];
            DataFormat  = (PvrDataFormat)TextureData[PvrtOffset + 0x09];

            // Get the codecs and make sure we can decode using them
            PixelCodec = PvrCodecList.GetPixelCodec(PixelFormat);
            if (PixelCodec == null) return false;

            DataCodec = PvrCodecList.GetDataCodec(DataFormat);
            if (DataCodec == null) return false;
            DataCodec.PixelCodec = PixelCodec;

            // Set the clut and data offsets
            if (DataCodec.ClutEntries == 0 || DataCodec.NeedsExternalClut)
            {
                ClutOffset = -1;
                DataOffset = PvrtOffset + 0x10;
            }
            else
            {
                ClutOffset = PvrtOffset + 0x10;
                DataOffset = ClutOffset + (DataCodec.ClutEntries * (PixelCodec.Bpp >> 3));
            }

            // Get the compression format and determine if we need to decompress this texture
            CompressionFormat = GetCompressionFormat(TextureData, PvrtOffset, DataOffset);
            CompressionCodec = PvrCodecList.GetCompressionCodec(CompressionFormat);

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

        #region Compression Format
        // Gets the compression format used on the PVR
        private PvrCompressionFormat GetCompressionFormat(byte[] data, int PvrtOffset, int DataOffset)
        {
            // RLE compression
            if (BitConverter.ToUInt32(data, 0x00) == BitConverter.ToUInt32(data, PvrtOffset + 4) - PvrtOffset + DataOffset + 8)
                return PvrCompressionFormat.Rle;

            return PvrCompressionFormat.None;
        }
        #endregion

        #region Texture Check
        /// <summary>
        /// Determines if this is a PVR texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">The offset in the byte array to start at.</param>
        /// <param name="length">Length of the data (in bytes).</param>
        /// <returns>True if this is a PVR texture, false otherwise.</returns>
        public static bool Is(byte[] source, int offset, int length)
        {
            // GBIX and PVRT
            if (length >= 0x20 &&
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("GBIX")) &&
                PTMethods.Contains(source, offset + 0x10, Encoding.UTF8.GetBytes("PVRT")) &&
                source[offset + 0x19] < 0x60 &&
                BitConverter.ToUInt32(source, offset + 0x14) == length - 24)
                return true;

            // PVRT (and no GBIX chunk)
            else if (length >= 0x10 &&
                PTMethods.Contains(source, offset + 0x00, Encoding.UTF8.GetBytes("PVRT")) &&
                source[offset + 0x09] < 0x60 &&
                BitConverter.ToUInt32(source, offset + 0x04) == length - 8)
                return true;

            // GBIX and PVRT with RLE compression
            else if (length >= 0x24 &&
                PTMethods.Contains(source, offset + 0x04, Encoding.UTF8.GetBytes("GBIX")) &&
                PTMethods.Contains(source, offset + 0x14, Encoding.UTF8.GetBytes("PVRT")) &&
                source[offset + 0x1D] < 0x60 &&
                BitConverter.ToUInt32(source, offset + 0x18) == BitConverter.ToUInt32(source, offset + 0x00) - 24)
                return true;

            // PVRT (and no GBIX chunk) with RLE compression 
            else if (length >= 0x14 &&
                PTMethods.Contains(source, offset + 0x04, Encoding.UTF8.GetBytes("PVRT")) &&
                source[offset + 0x0D] < 0x60 &&
                BitConverter.ToUInt32(source, offset + 0x08) == BitConverter.ToUInt32(source, offset + 0x00) - 8)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if this is a PVR texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>True if this is a PVR texture, false otherwise.</returns>
        public static bool Is(byte[] source)
        {
            return Is(source, 0, source.Length);
        }

        /// <summary>
        /// Determines if this is a PVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>True if this is a PVR texture, false otherwise.</returns>
        public static bool Is(Stream source, int length)
        {
            // If the length is < 16, then there is no way this is a valid texture.
            if (length < 16)
            {
                return false;
            }

            // Let's see if we should check 16 bytes, 32 bytes, or 36 bytes
            int amountToRead = 0;
            if (length < 32)
            {
                amountToRead = 16;
            }
            else if (length < 36)
            {
                amountToRead = 32;
            }
            else
            {
                amountToRead = 36;
            }

            byte[] buffer = new byte[amountToRead];
            source.Read(buffer, 0, amountToRead);
            source.Position -= amountToRead;

            return Is(buffer, 0, length);
        }

        /// <summary>
        /// Determines if this is a PVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a PVR texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            return Is(source, (int)(source.Length - source.Position));
        }
        #endregion
    }
}