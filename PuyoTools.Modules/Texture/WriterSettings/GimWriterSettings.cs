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
    public partial class GimWriterSettings : UserControl
    {
        public GimWriterSettings()
        {
            InitializeComponent();

            PaletteFormatBox.SelectedIndex = 0;
            DataFormatBox.SelectedIndex = 0;

            DataFormatBox_SelectedIndexChanged(null, null);
        }

        private void DataFormatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // The palette format box and has should only be enabled
            // if the data format is 4-bit Indexed or 8-bit Indexed
            PaletteFormatBox.Enabled = (DataFormatBox.SelectedIndex == 4 || DataFormatBox.SelectedIndex == 5);
        }
    }
}
