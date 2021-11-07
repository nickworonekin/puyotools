using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PuyoTools.Core.Textures.Gim;
using PuyoTools.GUI;
using PuyoTools.Core;
using GimTexture = PuyoTools.Core.Textures.GimTexture;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures;

namespace PuyoTools.App.Formats.Textures.WriterSettings
{
    public partial class GimWriterSettings : ModuleSettingsControl, ITextureFormatOptions
    {
        public GimWriterSettings()
        {
            InitializeComponent();

            paletteFormatBox.SelectedIndex = 0;
            dataFormatBox.SelectedIndex = 0;

            DataFormatBox_SelectedIndexChanged(null, null);
        }

        public void MapTo(TextureBase obj)
        {
            SetModuleSettings(obj);
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
                texture.PaletteFormat = null;
            }

            // Set the data format
            switch (dataFormatBox.SelectedIndex)
            {
                case 0: texture.DataFormat = GimPixelFormat.Rgb565; break;
                case 1: texture.DataFormat = GimPixelFormat.Argb1555; break;
                case 2: texture.DataFormat = GimPixelFormat.Argb4444; break;
                case 3: texture.DataFormat = GimPixelFormat.Argb8888; break;
                case 4: texture.DataFormat = GimPixelFormat.Index4; break;
                case 5: texture.DataFormat = GimPixelFormat.Index8; break;
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
