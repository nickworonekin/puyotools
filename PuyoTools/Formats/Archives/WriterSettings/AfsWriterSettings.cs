using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PuyoTools.GUI;
using PuyoTools.Modules;
using PuyoTools.Modules.Archive;

namespace PuyoTools.Formats.Archives.WriterSettings
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
            AfsArchiveWriter archive = (AfsArchiveWriter)module;

            archive.BlockSize = int.Parse(blockSizeBox.GetItemText(blockSizeBox.SelectedItem));

            if (afsVersion2Radio.Checked)
            {
                archive.Version = AfsArchiveWriter.AfsVersion.Version2;
            }
            else
            {
                archive.Version = AfsArchiveWriter.AfsVersion.Version1;
            }

            archive.HasTimestamps = hasTimestampsCheckbox.Checked;
        }
    }
}
