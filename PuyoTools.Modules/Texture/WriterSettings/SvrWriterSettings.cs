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
    public partial class SvrWriterSettings : UserControl
    {
        public SvrWriterSettings()
        {
            InitializeComponent();

            PixelFormatBox.SelectedIndex = 0;
            DataFormatBox.SelectedIndex = 0;
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
