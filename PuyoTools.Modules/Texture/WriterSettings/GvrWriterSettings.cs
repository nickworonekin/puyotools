using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.Modules.Texture
{
    public partial class GvrWriterSettings : UserControl
    {
        public GvrWriterSettings()
        {
            InitializeComponent();

            PaletteFormatBox.SelectedIndex = 2;
            DataFormatBox.SelectedIndex = 5;
            GbixTypeBox.SelectedIndex = 0;

            DataFormatBox_SelectedIndexChanged(null, null);
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
