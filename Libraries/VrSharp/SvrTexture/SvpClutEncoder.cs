using System;
using System.IO;
using System.Text;

namespace VrSharp.SvrTexture
{
    public class SvpClutEncoder : VpClutEncoder
    {
        #region Fields
        SvrPixelFormat PixelFormat; // Pixel Format
        #endregion

        #region Constructors
        /// <summary>
        /// Load a clut from a memory stream.
        /// </summary>
        /// <param name="stream">MemoryStream that contains the clut data.</param>
        /// <param name="NumClutEntries">Number of entries in the clut.</param>
        public SvpClutEncoder(MemoryStream stream, ushort NumClutEntries, SvrPixelFormat PixelFormat)
            : base(stream, NumClutEntries)
        {
            this.PixelFormat = PixelFormat;
        }

        /// <summary>
        /// Load a clut from a byte array.
        /// </summary>
        /// <param name="array">Byte array that contains the clut data.</param>
        public SvpClutEncoder(byte[] array, ushort NumClutEntries, SvrPixelFormat PixelFormat)
            : base(array, NumClutEntries)
        {
            this.PixelFormat = PixelFormat;
        }
        #endregion

        #region Clut
        public override byte[] WritePvplHeader()
        {
            MemoryStream PvplHeader = new MemoryStream();
            using (BinaryWriter Writer = new BinaryWriter(PvplHeader))
            {
                Writer.Write(Encoding.UTF8.GetBytes("PVPL"));
                Writer.Write(ClutData.Length + 8);
                Writer.Write((ushort)0x0000); // I don't know what this is for
                Writer.Write((ushort)PixelFormat);
                Writer.Write((ushort)0x0000);
                //Writer.Write(0x00000000); // Appears to be blank
                Writer.Write(ClutEntires);
                Writer.Flush();
            }

            return PvplHeader.ToArray();
        }
        #endregion
    }
}