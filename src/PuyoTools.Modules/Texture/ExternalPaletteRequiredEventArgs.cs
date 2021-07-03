using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuyoTools.Modules.Texture
{
    public class ExternalPaletteRequiredEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the palette data to use when decoding the texture.
        /// </summary>
        public Stream Palette { get; set; }

        /// <summary>
        /// Gets or sets whether to close the palette stream after being read. Defaults to false.
        /// </summary>
        public bool CloseAfterRead { get; set; }
    }
}
