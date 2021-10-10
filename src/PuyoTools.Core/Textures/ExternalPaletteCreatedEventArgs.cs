using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Core.Textures
{
    public class ExternalPaletteCreatedEventArgs : EventArgs
    {
        public ExternalPaletteCreatedEventArgs(Stream palette)
        {
            Palette = palette;
        }

        /// <summary>
        /// Gets the palette data that was created after encoding the texture.
        /// </summary>
        public Stream Palette { get; }
    }
}
