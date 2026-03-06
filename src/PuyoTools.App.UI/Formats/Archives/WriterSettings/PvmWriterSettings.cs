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
using PuyoTools.Archives.Formats.Gvm;
using PuyoTools.Archives.Formats.Pvm;
using PuyoTools.Archives.Formats.Svm;
using PuyoTools.Core;
using PuyoTools.Core.Archives;
using PuyoTools.GUI;

namespace PuyoTools.Formats.Archives.WriterSettings
{
    public partial class PvmWriterSettings : ModuleSettingsControl, IArchiveFormatOptions,
        IArchiveWriterOptions<GvmWriter>,
        IArchiveWriterOptions<PvmWriter>,
        IArchiveWriterOptions<SvmWriter>
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

        public void MapTo(GvmWriter obj)
        {
            obj.HasFilenames = hasFilenamesCheckbox.Checked;
            obj.HasGlobalIndexes = hasGlobalIndexesCheckbox.Checked;
            obj.HasFormats = hasFormatsCheckbox.Checked;
            obj.HasDimensions = hasDimensionsCheckbox.Checked;
        }

        public void MapTo(PvmWriter obj)
        {
            obj.HasFilenames = hasFilenamesCheckbox.Checked;
            obj.HasGlobalIndexes = hasGlobalIndexesCheckbox.Checked;
            obj.HasFormats = hasFormatsCheckbox.Checked;
            obj.HasDimensions = hasDimensionsCheckbox.Checked;
        }

        public void MapTo(SvmWriter obj)
        {
            obj.HasFilenames = hasFilenamesCheckbox.Checked;
            obj.HasGlobalIndexes = hasGlobalIndexesCheckbox.Checked;
            obj.HasFormats = hasFormatsCheckbox.Checked;
            obj.HasDimensions = hasDimensionsCheckbox.Checked;
        }

        private void MapTo(ArchiveWriter obj)
        {
            if (obj is GvmWriter gvmWriterObj)
            {
                MapTo(gvmWriterObj);
            }
            else if (obj is PvmWriter pvmWriterObj)
            {
                MapTo(pvmWriterObj);
            }
            else if (obj is SvmWriter svmWriterObj)
            {
                MapTo(svmWriterObj);
            }
        }

        void IArchiveWriterOptions.MapTo(ArchiveWriter obj) => MapTo(obj);

        void IMappable<ArchiveWriter>.MapTo(ArchiveWriter obj) => MapTo(obj);
    }
}
