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
using PuyoTools.Archives.Formats.Snt;

namespace PuyoTools.Formats.Archives.WriterSettings
{
    public partial class SntWriterSettings : ModuleSettingsControl, IArchiveFormatOptions,
        IArchiveWriterOptions<SntWriter>
    {
        public SntWriterSettings()
        {
            InitializeComponent();
        }

        public void MapTo(LegacyArchiveWriter obj)
        {
            SetModuleSettings(obj);
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

        public void MapTo(SntWriter obj)
        {
            if (platformPspRadio.Checked)
            {
                obj.Platform = SntPlatform.PlayStationPortable;
            }
            else
            {
                obj.Platform = SntPlatform.PlayStation2;
            }
        }
    }
}
