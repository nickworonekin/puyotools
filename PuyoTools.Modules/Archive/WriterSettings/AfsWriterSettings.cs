using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.Modules.Archive
{
    public partial class AfsWriterSettings : ModuleSettingsControl
    {
        public AfsWriterSettings()
        {
            InitializeComponent();

            blockSizeBox.SelectedIndex = 0;
        }

        public override void SetModuleSettings(IModule module)
        {
            AfsArchive.Writer archive = (AfsArchive.Writer)module;

            archive.BlockSize = int.Parse(blockSizeBox.GetItemText(blockSizeBox.SelectedItem));

            if (afsVersion2Radio.Checked)
            {
                archive.Version = AfsArchive.Writer.AfsVersion.Version2;
            }
            else
            {
                archive.Version = AfsArchive.Writer.AfsVersion.Version1;
            }
        }
    }
}
