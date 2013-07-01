using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using VrSharp.PvrTexture;

namespace PuyoTools.Modules.Texture
{
    public partial class PvrWriterSettings : ModuleSettingsControl
    {
        public PvrWriterSettings()
        {
            InitializeComponent();

            PixelFormatBox.SelectedIndex = 0;
            DataFormatBox.SelectedIndex = 0;
        }

        public override void SetModuleSettings(IModule module)
        {
            PvrTexture texture = (PvrTexture)module;

            // Set the pixel format
            switch (PixelFormatBox.SelectedIndex)
            {
                case 0: texture.PixelFormat = PvrPixelFormat.Argb1555; break;
                case 1: texture.PixelFormat = PvrPixelFormat.Rgb565; break;
                case 2: texture.PixelFormat = PvrPixelFormat.Argb4444; break;
            }

            // Set the data format
            switch (DataFormatBox.SelectedIndex)
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
            texture.HasGlobalIndex = HasGlobalIndexCheckBox.Checked;
            if (texture.HasGlobalIndex)
            {
                uint globalIndex;
                if (!uint.TryParse(GlobalIndexTextBox.Text, out globalIndex))
                {
                    globalIndex = 0;
                }

                texture.GlobalIndex = globalIndex;
            }

            // RLE compress the texture?
            if (RleCompressionCheckBox.Checked)
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
            GlobalIndexTextBox.Enabled = HasGlobalIndexCheckBox.Checked;
        }
    }
}
