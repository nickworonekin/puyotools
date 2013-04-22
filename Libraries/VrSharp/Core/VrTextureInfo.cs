using System;

namespace VrSharp
{
    public abstract class VrTextureInfo
    {
        /// <summary>
        /// Width of the texture
        /// </summary>
        public ushort TextureWidth;
        /// <summary>
        /// Height of the texture
        /// </summary>
        public ushort TextureHeight;

        /// <summary>
        /// Pixel Format (0xFF = No Pixel Format)
        /// </summary>
        public byte PixelFormat;
        /// <summary>
        /// Data Format
        /// </summary>
        public byte DataFormat;

        /// <summary>
        /// Global Index
        /// </summary>
        public uint GlobalIndex;

        /// <summary>
        /// Offset of the PVRT (or GVRT) chunk.
        /// </summary>
        public int PvrtOffset;
    }
}