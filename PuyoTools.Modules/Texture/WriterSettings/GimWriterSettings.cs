using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GimSharp;

namespace PuyoTools.Modules.Texture
{
    public partial class GimWriterSettings : ModuleSettingsControl
    {
        public GimWriterSettings()
        {
            InitializeComponent();

            paletteFormatBox.SelectedIndex = 0;
            dataFormatBox.SelectedIndex = 0;

            DataFormatBox_SelectedIndexChanged(null, null);
        }

        public override void SetModuleSettings(IModule module)
        {
            GimTexture texture = (GimTexture)module;

            bool hasPaletteFormat = (dataFormatBox.SelectedIndex == 4 || dataFormatBox.SelectedIndex == 5);

            // Set the palette format
            if (hasPaletteFormat)
            {
                switch (paletteFormatBox.SelectedIndex)
                {
                    case 0: texture.PaletteFormat = GimPaletteFormat.Rgb565; break;
                    case 1: texture.PaletteFormat = GimPaletteFormat.Argb1555; break;
                    case 2: texture.PaletteFormat = GimPaletteFormat.Argb4444; break;
                    case 3: texture.PaletteFormat = GimPaletteFormat.Argb8888; break;
                }
            }
            else
            {
                texture.PaletteFormat = GimPaletteFormat.Unknown;
            }

            // Set the data format
            switch (dataFormatBox.SelectedIndex)
            {
                case 0: texture.DataFormat = GimDataFormat.Rgb565; break;
                case 1: texture.DataFormat = GimDataFormat.Argb1555; break;
                case 2: texture.DataFormat = GimDataFormat.Argb4444; break;
                case 3: texture.DataFormat = GimDataFormat.Argb8888; break;
                case 4: texture.DataFormat = GimDataFormat.Index4; break;
                case 5: texture.DataFormat = GimDataFormat.Index8; break;
            }

            // Has metadata?
            texture.HasMetadata = hasMetadataCheckBox.Checked;
        }

        private void DataFormatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // The palette format box and has should only be enabled
            // if the data format is 4-bit Indexed or 8-bit Indexed
            paletteFormatBox.Enabled = (dataFormatBox.SelectedIndex == 4 || dataFormatBox.SelectedIndex == 5);
        }
    }
}
