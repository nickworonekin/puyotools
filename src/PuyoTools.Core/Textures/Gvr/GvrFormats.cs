using System;

namespace PuyoTools.Core.Textures.Gvr
{
    // Gvr Data Flags
    [Flags]
    public enum GvrDataFlags : byte
    {
        None            = 0x0,
        Mipmaps         = 0x1,
        ExternalPalette = 0x2,
        InternalPalette = 0x8,
        Palette         = ExternalPalette | InternalPalette,
    }

    public enum GvrGbixType
    {
        None,

        /// <summary>
        /// A magic code of "GBIX". This is generally used for textures in GameCube games.
        /// </summary>
        Gbix,

        /// <summary>
        /// A magic code of "GCIX". This is generally used for textures in Wii games.
        /// </summary>
        Gcix,
    }
}