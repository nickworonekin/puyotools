using System;
using System.IO;

namespace VrSharp.SvrTexture
{
    public class SvpClut : VpClut
    {
        #region Constructors
        /// <summary>
        /// Open a Svp clut from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the clut data.</param>
        public SvpClut(string file)
            : base(file)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        public SvpClut(Stream stream)
            : base(stream)
        {
            InitSuccess = ReadHeader();
        }

        /// <summary>
        /// Open a Svp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        public SvpClut(byte[] array)
            : base(array)
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

            NumClutEntries = BitConverter.ToUInt16(ClutData, 0x0E);

            // Get the correct pixel codec from the clut file.
            // If we don't know the format in the clut file then the
            // one specified in the texture file should be used instead.
            PixelCodec = SvrCodecList.GetPixelCodec((SvrPixelFormat)PixelFormat);

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