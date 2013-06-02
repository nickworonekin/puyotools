using System;
using System.IO;

namespace VrSharp.PvrTexture
{
    public class PvpPalette : VpPalette
    {
        #region Constructors
        /// <summary>
        /// Open a Pvp clut from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the clut data.</param>
        public PvpPalette(string file)
            : base(file)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Pvp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        public PvpPalette(Stream stream)
            : base(stream)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Pvp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public PvpPalette(Stream stream, int length)
            : base(stream, length)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Pvp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        public PvpPalette(byte[] array)
            : base(array)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Pvp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        /// <param name="offset">Offset of the clut data in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public PvpPalette(byte[] array, long offset, int length)
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
            if (!IsPvpClut(ClutData))
                return false;

            NumPaletteEntries = BitConverter.ToUInt16(ClutData, 0x0E);

            // Get the correct pixel codec from the clut file.
            // Sometimes, the pixel codec specified in the texture file
            // is not the same one specified in the clut file. As such, we should
            // always use the one specified in the clut file.
            PixelCodec = PvrPixelCodec.GetPixelCodec((PvrPixelFormat)ClutData[0x08]);

            return true;
        }

        // Checks if the input file is a pvp
        private bool IsPvpClut(byte[] data)
        {
            if (Compare(data, "PVPL", 0x00) &&
                BitConverter.ToUInt32(data, 0x04) == data.Length - 8)
                return true;

            return false;
        }
        #endregion
    }
}