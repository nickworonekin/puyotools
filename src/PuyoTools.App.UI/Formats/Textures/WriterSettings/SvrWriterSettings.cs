using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PuyoTools.Core.Textures.Svr;
using PuyoTools.GUI;
using PuyoTools.App.Formats.Textures;
using PuyoTools.Core.Textures;
using PuyoTools.Core;
using SvrTexture = PuyoTools.Core.Textures.SvrTexture;

namespace PuyoTools.App.Formats.Textures.WriterSettings
{
    public partial class SvrWriterSettings : ModuleSettingsControl, ITextureFormatOptions
    {
        public SvrWriterSettings()
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
            SvrTexture texture = (SvrTexture)module;

            // Set the pixel format
            switch (pixelFormatBox.SelectedIndex)
            {
                case 0: texture.PixelFormat = SvrPixelFormat.Rgb5a3; break;
                case 1: texture.PixelFormat = SvrPixelFormat.Argb8888; break;
            }

            // Set the data format
            switch (dataFormatBox.SelectedIndex)
            {
                case 0: texture.DataFormat = SvrDataFormat.Rectangle; break;
                case 1: texture.DataFormat = SvrDataFormat.Index4ExternalPalette; break;
                case 2: texture.DataFormat = SvrDataFormat.Index8ExternalPalette; break;
                case 3: texture.DataFormat = SvrDataFormat.Index4; break;
                case 4: texture.DataFormat = SvrDataFormat.Index8; break;
            }

            // Set the global index stuff
            texture.HasGlobalIndex = hasGlobalIndexCheckBox.Checked;
            if (texture.HasGlobalIndex)
            {
                uint globalIndex = 0;
                if (!uint.TryParse(globalIndexTextBox.Text, out globalIndex))
                {
                    globalIndex = 0;
                }
                texture.GlobalIndex = globalIndex;
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
