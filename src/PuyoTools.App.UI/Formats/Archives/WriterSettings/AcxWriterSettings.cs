using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PuyoTools.App;
using PuyoTools.App.Formats.Archives;
using PuyoTools.Archives;
using PuyoTools.Archives.Formats.Acx;
using PuyoTools.Core;
using PuyoTools.Core.Archives;
using PuyoTools.GUI;

namespace PuyoTools.Formats.Archives.WriterSettings
{
    public partial class AcxWriterSettings : ModuleSettingsControl, IArchiveFormatOptions,
        IArchiveWriterOptions<AcxWriter>
    {
        public AcxWriterSettings()
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
            AcxArchiveWriter archive = (AcxArchiveWriter)module;

            archive.BlockSize = int.Parse(blockSizeBox.GetItemText(blockSizeBox.SelectedItem));
        }

        public void MapTo(AcxWriter obj)
        {
            obj.BlockSize = int.Parse(blockSizeBox.GetItemText(blockSizeBox.SelectedItem) ?? string.Empty);
        }
    }
}
