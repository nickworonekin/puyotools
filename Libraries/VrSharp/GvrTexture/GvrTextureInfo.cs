using System;

namespace VrSharp.GvrTexture
{
    public class GvrTextureInfo : VrTextureInfo
    {

        /// <summary>
        /// <para>Data Flags:</para>
        /// <para>Mipmaps (0x1) - Texture contains mipmaps</para>
        /// <para>ExternalClut (0x2) - Texture contains a CLUT that is stored in a separate file</para>
        /// <para>InternalClut (0x8) - Texture contains a CLUT that is stored in the texture file</para>
        /// </summary>
        public GvrDataFlags DataFlags;
    }
}