using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PuyoTools.GUI;
using PuyoTools.Core;
using PuyoTools.Core.Archives;
using PuyoTools.App.Formats.Archives;

namespace PuyoTools.Formats.Archives.WriterSettings
{
    public partial class AfsWriterSettings : ModuleSettingsControl, IArchiveFormatOptions
    {
        public AfsWriterSettings()
        {
            InitializeComponent();

            blockSizeBox.SelectedIndex = 0;
        }

        public void MapTo(ArchiveWriter obj)
        {
            SetModuleSettings(obj);
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
