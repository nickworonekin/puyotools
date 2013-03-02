using System;
using System.IO;

namespace VrSharp
{
    public abstract class VpClutEncoder
    {
        #region Fields
        protected bool InitSuccess = false; // Initalization

        protected byte[] ClutData;    // Clut Data
        protected ushort ClutEntires; // Num of Entries in the Clut
        #endregion

        #region Constructors
        /// <summary>
        /// Load a clut from a memory stream.
        /// </summary>
        /// <param name="stream">MemoryStream that contains the clut data.</param>
        /// <param name="NumClutEntries">Number of entries in the clut.</param>
        public VpClutEncoder(MemoryStream stream, ushort NumClutEntries)
        {
            stream.Seek(0, SeekOrigin.Begin); // Seek to the beginning
            try { ClutData = stream.ToArray(); }
            catch
            {
                ClutData    = new byte[0];
                InitSuccess = false;
                return;
            }

            ClutEntires = NumClutEntries;
            InitSuccess = true;
        }

        /// <summary>
        /// Load a clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        /// <param name="NumClutEntries">Number of entries in the clut.</param>
        public VpClutEncoder(byte[] array, ushort NumClutEntries)
        {
            if (array != null)
                ClutData = array;
            else
            {
                ClutData    = new byte[0];
                InitSuccess = false;
                return;
            }

            ClutEntires = NumClutEntries;
            InitSuccess = true;
        }
        #endregion

        #region Clut
        /// <summary>
        /// Returns the clut as an array (clone of GetClutAsArray).
        /// </summary>
        /// <returns></returns>
        public byte[] GetClut()
        {
            return GetClutAsArray();
        }

        /// <summary>
        /// Returns the clut as an array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetClutAsArray()
        {
            if (!InitSuccess) return null;

            return EncodeClut();
        }

        /// <summary>
        /// Returns the clut an a memory stream.
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetClutAsStream()
        {
            if (!InitSuccess) return null;

            return new MemoryStream(EncodeClut());
        }

        public abstract byte[] WritePvplHeader();
        #endregion

        #region Misc
        // Swap endian of a 16-bit unsigned integer (a ushort)
        protected ushort SwapUShort(ushort x)
        {
            return (ushort)((x << 8) | (x >> 8));
        }
        #endregion

        #region Private Methods
        private byte[] EncodeClut()
        {
            byte[] VpClutData = new byte[0x10 + ClutData.Length];

            byte[] PvplHeader = WritePvplHeader();
            PvplHeader.CopyTo(VpClutData, 0x00);
            ClutData.CopyTo(VpClutData, 0x10);

            return VpClutData;
        }
        #endregion
    }
}