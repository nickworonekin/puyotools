using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using VrSharp.GvrTexture;

namespace PuyoTools.Modules.Texture
{
    public partial class GvrWriterSettings : ModuleSettingsControl
    {
        public GvrWriterSettings()
        {
            InitializeComponent();

            PaletteFormatBox.SelectedIndex = 2;
            DataFormatBox.SelectedIndex = 5;
            GbixTypeBox.SelectedIndex = 0;

            DataFormatBox_SelectedIndexChanged(null, null);
        }

        public override void SetModuleSettings(IModule module)
        {
            GvrTexture texture = (GvrTexture)module;

            bool hasPaletteFormat = (DataFormatBox.SelectedIndex == 7 || DataFormatBox.SelectedIndex == 8);
            bool canHaveMipmaps = (DataFormatBox.SelectedIndex == 4 || DataFormatBox.SelectedIndex == 5 || DataFormatBox.SelectedIndex == 9);

            // Set the palette format
            if (PaletteFormatBox.Enabled)
            {
                switch (PaletteFormatBox.SelectedIndex)
                {
                    case 0: texture.PaletteFormat = GvrPixelFormat.IntensityA8; break;
                    case 1: texture.PaletteFormat = GvrPixelFormat.Rgb565; break;
                    case 2: texture.PaletteFormat = GvrPixelFormat.Rgb5a3; break;
                }
            }
            else
            {
                texture.PaletteFormat = GvrPixelFormat.Unknown;
            }

            // Set the data format
            switch (DataFormatBox.SelectedIndex)
            {
                case 0: texture.DataFormat = GvrDataFormat.Intensity4; break;
                case 1: texture.DataFormat = GvrDataFormat.Intensity8; break;
                case 2: texture.DataFormat = GvrDataFormat.IntensityA4; break;
                case 3: texture.DataFormat = GvrDataFormat.IntensityA8; break;
                case 4: texture.DataFormat = GvrDataFormat.Rgb565; break;
                case 5: texture.DataFormat = GvrDataFormat.Rgb5a3; break;
                case 6: texture.DataFormat = GvrDataFormat.Argb8888; break;
                case 7: texture.DataFormat = GvrDataFormat.Index4; break;
                case 8: texture.DataFormat = GvrDataFormat.Index8; break;
                case 9: texture.DataFormat = GvrDataFormat.Dxt1; break;
            }

            // Set the global index, the global index type, and if it has a global index
            texture.HasGlobalIndex = HasGlobalIndexCheckBox.Checked;
            if (texture.HasGlobalIndex)
            {
                uint globalIndex;
                if (!uint.TryParse(GlobalIndexTextBox.Text, out globalIndex))
                {
                    globalIndex = 0;
                }
                texture.GlobalIndex = globalIndex;

                switch (GbixTypeBox.SelectedIndex)
                {
                    case 0: texture.GbixType = GvrGbixType.Gbix; break;
                    case 1: texture.GbixType = GvrGbixType.Gcix; break;
                }
            }

            // Has mipmaps?
            texture.HasMipmaps = (canHaveMipmaps && HasMipmapsCheckBox.Checked);

            // Has external palette?
            texture.NeedsExternalPalette = (hasPaletteFormat && HasExternalPaletteCheckBox.Checked);
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
            GbixTypeBox.Enabled = HasGlobalIndexCheckBox.Checked;
        }

        private void DataFormatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // The palette format box and has external palette checkbox should only be enabled
            // if the data format is 4-bit Indexed or 8-bit Indexed
            PaletteFormatBox.Enabled = (DataFormatBox.SelectedIndex == 7 || DataFormatBox.SelectedIndex == 8);
            HasExternalPaletteCheckBox.Enabled = PaletteFormatBox.Enabled;

            // The has mipmaps checkbox should only be enabled if the data format's bpp is 4 or 16 and is
            // not a palettized format.
            // But at the current moment, we're not going to enable it for the intensity formats.
            HasMipmapsCheckBox.Enabled = (DataFormatBox.SelectedIndex == 4 || DataFormatBox.SelectedIndex == 5 || DataFormatBox.SelectedIndex == 9);
        }
    }
}
