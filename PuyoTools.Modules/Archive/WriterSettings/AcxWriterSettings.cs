using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.Modules.Archive
{
    public partial class AcxWriterSettings : ModuleSettingsControl
    {
        public AcxWriterSettings()
        {
            InitializeComponent();

            blockSizeBox.SelectedIndex = 0;
        }

        public override void SetModuleSettings(IModule module)
        {
            AcxArchiveWriter archive = (AcxArchiveWriter)module;

            archive.BlockSize = int.Parse(blockSizeBox.GetItemText(blockSizeBox.SelectedItem));
        }
    }
}
