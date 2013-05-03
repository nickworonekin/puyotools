using System;
using System.IO;
using System.Drawing;

namespace VrSharp.SvrTexture
{
    public class SvrTexture : VrTexture
    {
        #region Texture Properties
        /// <summary>
        /// The texture's pixel format.
        /// </summary>
        public SvrPixelFormat PixelFormat { get; private set; }

        /// <summary>
        /// The texture's data format.
        /// </summary>
        public SvrDataFormat DataFormat { get; private set; }
        #endregion

        #region Constructors & Initalizers
        /// <summary>
        /// Open a SVR texture from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the texture data.</param>
        public SvrTexture(string file) : base(file) { }

        /// <summary>
        /// Open a SVR texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        public SvrTexture(byte[] source) : base(source) { }

        /// <summary>
        /// Open a SVR texture from a byte array.
        /// </summary>
        /// <param name="source">Byte array that contains the texture data.</param>
        /// <param name="offset">Offset of the texture in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public SvrTexture(byte[] source, long offset, int length) : base(source, (int)offset, length) { }

        /// <summary>
        /// Open a SVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        public SvrTexture(Stream source) : base(source) { }

        /// <summary>
        /// Open a SVR texture from a stream.
        /// </summary>
        /// <param name="source">Stream that contains the texture data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public SvrTexture(Stream source, int length) : base(source, length) { }

        protected override bool Initalize()
        {
            // Check to see if what we are dealing with is a SVR texture
            if (!Is(TextureData))
                return false;

            // Determine the offsets of the GBIX (if present) and PVRT header chunks.
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

            PixelFormat = (SvrPixelFormat)TextureData[PvrtOffset + 0x08];
            DataFormat  = (SvrDataFormat)TextureData[PvrtOffset + 0x09];

            // Get the codecs and make sure we can decode using them
            PixelCodec = SvrCodecList.GetPixelCodec(PixelFormat);
            if (PixelCodec == null || !PixelCodec.CanDecode()) return false;

            DataCodec = SvrCodecList.GetDataCodec(DataFormat);
            if (DataCodec == null || !DataCodec.CanDecode()) return false;

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

        #region Texture Check
        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="offset">The offset in the byte array to start at.</param>
        /// <param name="length">Length of the data (in bytes).</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
        public static bool Is(byte[] source, int offset, int length)
        {
            // GBIX and PVRT
            if (length >= 0x20 &&
                Compare(source, "GBIX", offset + 0x00) &&
                Compare(source, "PVRT", offset + 0x10) &&
                source[offset + 0x19] >= 0x60 && source[offset + 0x19] < 0x70 &&
                BitConverter.ToUInt32(source, offset + 0x14) == length - 24)
                return true;

            // PVRT (and no GBIX chunk)
            else if (length >= 0x10 &&
                Compare(source, "PVRT", offset + 0x00) &&
                source[offset + 0x19] >= 0x60 && source[offset + 0x19] < 0x70 &&
                BitConverter.ToUInt32(source, offset + 0x04) == length - 8)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
        public static bool Is(byte[] source)
        {
            return Is(source, 0, source.Length);
        }

        /// <summary>
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
        public static bool Is(Stream source, int length)
        {
            // If the length is < 16, then there is no way this is a valid texture.
            if (length < 16)
            {
                return false;
            }

            // Let's see if we should check 16 bytes or 32 bytes
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
        /// Determines if this is a SVR texture.
        /// </summary>
        /// <param name="source">The stream to read from. The stream position is not changed.</param>
        /// <returns>True if this is a SVR texture, false otherwise.</returns>
        public static bool Is(Stream source)
        {
            return Is(source, (int)(source.Length - source.Position));
        }
        #endregion
    }
}