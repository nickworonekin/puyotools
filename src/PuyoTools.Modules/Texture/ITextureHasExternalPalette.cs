using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Modules.Texture
{
    /// <summary>
    /// Represents a texture that may require the use of an external palette file.
    /// </summary>
    public interface ITextureHasExternalPalette
    {
        /// <summary>
        /// Gets or sets the external palette data for this texture.
        /// </summary>
        /// <remarks>
        /// <para>When decoding, if this is null and the texture requires an external palette, <see cref="TextureNeedsPaletteException"/> will be thrown.</para>
        /// <para>When encoding, this will contain the palette data if the texture uses an external palette.</para>
        /// </remarks>
        Stream PaletteStream { get; set; }
    }
}
