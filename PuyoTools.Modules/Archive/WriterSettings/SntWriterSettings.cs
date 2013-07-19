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
    public partial class SntWriterSettings : ModuleSettingsControl
    {
        public SntWriterSettings()
        {
            InitializeComponent();
        }

        public override void SetModuleSettings(IModule module)
        {
            SntArchiveWriter archive = (SntArchiveWriter)module;

            if (platformPspRadio.Checked)
            {
                archive.Platform = SntArchiveWriter.SntPlatform.Psp;
            }
            else
            {
                archive.Platform = SntArchiveWriter.SntPlatform.Ps2;
            }
        }
    }
}
