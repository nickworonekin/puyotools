using System;
using System.IO;

namespace VrSharp.GvrTexture
{
    public class GvpPalette : VpPalette
    {
        #region Constructors
        /// <summary>
        /// Open a Gvp clut from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the clut data.</param>
        public GvpPalette(string file)
            : base(file)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Gvp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        public GvpPalette(Stream stream)
            : base(stream)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Gvp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GvpPalette(Stream stream, int length)
            : base(stream, length)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Gvp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        public GvpPalette(byte[] array)
            : base(array)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Gvp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        /// <param name="offset">Offset of the clut data in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public GvpPalette(byte[] array, long offset, int length)
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
            if (!IsGvpClut(ClutData))
                return false;

            NumPaletteEntries = (ushort)((ClutData[0x0E] << 8) | ClutData[0x0F]);

            // I don't know how gvp's are supposed to be formatted
            PixelFormat = (byte)GvrPixelFormat.Unknown;
            PixelCodec  = null;

            return true;
        }

        // Checks if the input file is a gvp
        private bool IsGvpClut(byte[] data)
        {
            if (Compare(data, "GVPL", 0x00) &&
                BitConverter.ToUInt32(data, 0x04) == data.Length - 8)
                return true;

            return false;
        }
        #endregion
    }
}