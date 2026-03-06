using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PuyoTools.App;
using PuyoTools.App.Formats.Archives;
using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Afs;
using PuyoTools.Core;
using PuyoTools.Core.Archives;
using PuyoTools.GUI;

namespace PuyoTools.Formats.Archives.WriterSettings
{
    public partial class AfsWriterSettings : ModuleSettingsControl, IArchiveFormatOptions,
        IArchiveWriterOptions<AfsWriter>
    {
        public AfsWriterSettings()
        {
            InitializeComponent();

            blockSizeBox.SelectedIndex = 0;
        }

        public void MapTo(LegacyArchiveWriter obj)
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

        public void MapTo(AfsWriter obj)
        {
            obj.BlockSize = int.Parse(blockSizeBox.GetItemText(blockSizeBox.SelectedItem) ?? string.Empty);

            if (afsVersion2Radio.Checked)
            {
                obj.Version = AfsVersion.Version2;
            }
            else
            {
                obj.Version = AfsVersion.Version1;
            }

            obj.HasTimestamps = hasTimestampsCheckbox.Checked;
        }
    }
}
