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
            SntArchive.Writer archive = (SntArchive.Writer)module;

            if (platformPspRadio.Checked)
            {
                archive.Platform = SntArchive.Writer.SntPlatform.Psp;
            }
            else
            {
                archive.Platform = SntArchive.Writer.SntPlatform.Ps2;
            }
        }
    }
}
