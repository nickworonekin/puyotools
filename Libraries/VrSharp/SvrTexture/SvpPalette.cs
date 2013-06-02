using System;
using System.IO;

namespace VrSharp.SvrTexture
{
    public class SvpPalette : VpPalette
    {
        #region Constructors
        /// <summary>
        /// Open a Svp clut from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the clut data.</param>
        public SvpPalette(string file)
            : base(file)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        public SvpPalette(Stream stream)
            : base(stream)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public SvpPalette(Stream stream, int length)
            : base(stream, length)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        public SvpPalette(byte[] array)
            : base(array)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        /// <param name="offset">Offset of the clut data in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public SvpPalette(byte[] array, long offset, int length)
            : base(array, offset, length)
        {
            InitSuccess = ReadHeader();
        }
        #endregion

        #region Header
        // Read the header and sets up the appropiate values.
        // Returns true if successful, otherwise false
        private bool ReadHeader()
        {
            if (!IsSvpClut(ClutData))
                return false;

            NumPaletteEntries = BitConverter.ToUInt16(ClutData, 0x0E);

            // Get the correct pixel codec from the clut file.
            // Sometimes, the pixel codec specified in the texture file
            // is not the same one specified in the clut file. As such, we should
            // always use the one specified in the clut file.
            PixelCodec = SvrPixelCodec.GetPixelCodec((SvrPixelFormat)ClutData[0x08]);

            return true;
        }

        // Checks if the input file is a svp
        private bool IsSvpClut(byte[] data)
        {
            if (Compare(data, "PVPL", 0x00) &&
                BitConverter.ToUInt32(data, 0x04) == data.Length - 8)
                return true;

            return false;
        }
        #endregion
    }
}