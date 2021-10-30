using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Textures
{
    /// <summary>
    /// Represents a texture that may require the use of an external palette file.
    /// </summary>
    public interface ITextureHasExternalPalette
    {
        /// <summary>
        /// Occurs when an external palette is required to decode this texture.
        /// </summary>
        event EventHandler<ExternalPaletteRequiredEventArgs> ExternalPaletteRequired;

        /// <summary>
        /// Occurs when an external palette is created after encoding this texture.
        /// </summary>
        event EventHandler<ExternalPaletteCreatedEventArgs> ExternalPaletteCreated;
    }
}
