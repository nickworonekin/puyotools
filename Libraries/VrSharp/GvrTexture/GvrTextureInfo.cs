using System;

namespace VrSharp.GvrTexture
{
    public class GvrTextureInfo : VrTextureInfo
    {
        /// <summary>
        /// Data Flags:
        /// 0x01: Contains Mipmaps;
        /// 0x02: Contains External Clut;
        /// 0x08: Contains Internal Clut
        /// </summary>
        public byte DataFlags;
    }
}