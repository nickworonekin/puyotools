using PuyoTools.GUI;
using PuyoTools.Modules;
using PuyoTools.Modules.Archive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.Formats.Archives.WriterSettings
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
