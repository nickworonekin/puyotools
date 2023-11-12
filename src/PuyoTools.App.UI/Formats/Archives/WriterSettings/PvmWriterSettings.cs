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
    public partial class PvmWriterSettings : ModuleSettingsControl, IArchiveFormatOptions
    {
        public PvmWriterSettings()
        {
            InitializeComponent();
        }

        public void MapTo(LegacyArchiveWriter obj)
        {
            SetModuleSettings(obj);
        }

        public override void SetModuleSettings(IModule module)
        {
            // GVM archive
            if (module is GvmArchiveWriter)
            {
                GvmArchiveWriter archive = (GvmArchiveWriter)module;

                archive.HasFilenames = hasFilenamesCheckbox.Checked;
                archive.HasGlobalIndexes = hasGlobalIndexesCheckbox.Checked;
                archive.HasFormats = hasFormatsCheckbox.Checked;
                archive.HasDimensions = hasDimensionsCheckbox.Checked;
            }

            // PVM archive
            else if (module is PvmArchiveWriter)
            {
                PvmArchiveWriter archive = (PvmArchiveWriter)module;

                archive.HasFilenames = hasFilenamesCheckbox.Checked;
                archive.HasGlobalIndexes = hasGlobalIndexesCheckbox.Checked;
                archive.HasFormats = hasFormatsCheckbox.Checked;
                archive.HasDimensions = hasDimensionsCheckbox.Checked;
            }

            // SVM archive
            else if (module is SvmArchiveWriter)
            {
                SvmArchiveWriter archive = (SvmArchiveWriter)module;

                archive.HasFilenames = hasFilenamesCheckbox.Checked;
                archive.HasGlobalIndexes = hasGlobalIndexesCheckbox.Checked;
                archive.HasFormats = hasFormatsCheckbox.Checked;
                archive.HasDimensions = hasDimensionsCheckbox.Checked;
            }
        }
    }
}
