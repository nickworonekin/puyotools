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
    public partial class PvmWriterSettings : ModuleSettingsControl
    {
        public PvmWriterSettings()
        {
            InitializeComponent();
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
        }
    }
}
