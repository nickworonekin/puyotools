using System;
using System.IO;

namespace VrSharp
{
    public abstract class VpPalette
    {
        #region Fields
        protected bool InitSuccess = false; // Initalization

        protected byte[] ClutData; // Vp Palette Data

        protected ushort NumPaletteEntries; // Number of Palette Entries

        protected byte PixelFormat;     // Pixel Format
        public VrPixelCodec PixelCodec; // Pixel Codec
        #endregion

        #region Constructors
        /// <summary>
        /// Open a Vp clut from a file.
        /// </summary>
        /// <param name="file">Filename of the file that contains the clut data.</param>
        public VpPalette(string file)
        {
            byte[] data;
            try
            {
                data = File.ReadAllBytes(file);
            }
            catch { data = new byte[0]; }

            ClutData = data;
        }

        /// <summary>
        /// Open a Vp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        public VpPalette(Stream stream) : this(stream, (int)(stream.Length - stream.Position)) { }

        /// <summary>
        /// Open a Vp clut from a stream.
        /// </summary>
        /// <param name="stream">Stream that contains the clut data.</param>
        /// <param name="length">Number of bytes to read.</param>
        public VpPalette(Stream stream, int length)
        {
            byte[] data;
            try
            {
                data = new byte[length];
                stream.Read(data, 0, length);
            }
            catch { data = new byte[0]; }

            ClutData = data;
        }

        /// <summary>
        /// Open a Vp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        public VpPalette(byte[] array) : this(array, 0, array.Length) { }

        /// <summary>
        /// Open a Vp clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        /// <param name="offset">Offset of the clut data in the array.</param>
        /// <param name="length">Number of bytes to read.</param>
        public VpPalette(byte[] array, long offset, int length)
        {
            byte[] data;
            if (array == null)
                data = new byte[0];
            else
            {
                data = new byte[length];
                try
                {
                    Array.Copy(array, offset, data, 0, length);
                }
                catch { data = new byte[0]; }
            }

            ClutData = data;
        }
        #endregion

        #region Palette
        /// <summary>
        /// Get the palette data.
        /// </summary>
        /// <param name="PixelCodec">Pixel Codec used for the clut.</param>
        /// <returns></returns>
        public byte[] GetPalette(VrPixelCodec PixelCodec)
        {
            if (!InitSuccess) return new byte[0];

            byte[] clut = new byte[NumPaletteEntries * (PixelCodec.Bpp >> 3)];
            Array.Copy(ClutData, 0x10, clut, 0x00, clut.Length);

            return clut;
        }

        /// <summary>
        /// Get the number of entries in the palette file.
        /// </summary>
        /// <returns></returns>
        public ushort GetNumPaletteEntries()
        {
            if (!InitSuccess) return 0;
            return NumPaletteEntries;
        }
        #endregion

        #region Misc
        /// <summary>
        /// Returns if the palette was loaded successfully.
        /// </summary>
        /// <returns></returns>
        public bool LoadSuccess()
        {
            return InitSuccess;
        }
        #endregion

        #region Private Properties
        // Function for checking headers
        // Checks to see if the string matches the byte data at the specific offset
        protected static bool Compare(byte[] array, string str, int offset)
        {
            if (offset < 0 || offset + str.Length > array.Length)
                return false; // Out of bounds

            for (int i = 0; i < str.Length; i++)
            {
                if (array[offset + i] != str[i])
                    return false;
            }

            return true;
        }
        #endregion
    }
}