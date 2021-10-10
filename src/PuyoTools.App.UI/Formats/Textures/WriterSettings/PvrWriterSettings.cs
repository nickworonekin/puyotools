using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PuyoTools.Core.Textures.Pvr;
using PuyoTools.GUI;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures;
using PuyoTools.Core;
using PvrTexture = PuyoTools.Core.Textures.PvrTexture;

namespace PuyoTools.App.Formats.Textures.WriterSettings
{
    public partial class PvrWriterSettings : ModuleSettingsControl, ITextureFormatOptions
    {
        public PvrWriterSettings()
        {
            InitializeComponent();

            pixelFormatBox.SelectedIndex = 0;
            dataFormatBox.SelectedIndex = 0;
        }

        public void MapTo(TextureBase obj)
        {
            SetModuleSettings(obj);
        }

        public override void SetModuleSettings(IModule module)
        {
            PvrTexture texture = (PvrTexture)module;

            // Set the pixel format
            switch (pixelFormatBox.SelectedIndex)
            {
                case 0: texture.PixelFormat = PvrPixelFormat.Argb1555; break;
                case 1: texture.PixelFormat = PvrPixelFormat.Rgb565; break;
                case 2: texture.PixelFormat = PvrPixelFormat.Argb4444; break;
            }

            // Set the data format
            switch (dataFormatBox.SelectedIndex)
            {
                case 0: texture.DataFormat = PvrDataFormat.SquareTwiddled; break;
                case 1: texture.DataFormat = PvrDataFormat.SquareTwiddledMipmaps; break;
                case 2: texture.DataFormat = PvrDataFormat.Index4; break;
                case 3: texture.DataFormat = PvrDataFormat.Index8; break;
                case 4: texture.DataFormat = PvrDataFormat.Rectangle; break;
                case 5: texture.DataFormat = PvrDataFormat.RectangleTwiddled; break;
                case 6: texture.DataFormat = PvrDataFormat.SquareTwiddledMipmapsAlt; break;
            }

            // Set the global index and if it has a global index
            texture.HasGlobalIndex = hasGlobalIndexCheckBox.Checked;
            if (texture.HasGlobalIndex)
            {
                uint globalIndex;
                if (!uint.TryParse(globalIndexTextBox.Text, out globalIndex))
                {
                    globalIndex = 0;
                }

                texture.GlobalIndex = globalIndex;
            }

            // RLE compress the texture?
            if (rleCompressionCheckBox.Checked)
            {
                texture.CompressionFormat = PvrCompressionFormat.Rle;
            }
        }

        private void globalIndexTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Only integers are allowed
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void hasGlobalIndexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            globalIndexTextBox.Enabled = hasGlobalIndexCheckBox.Checked;
        }
    }
}
