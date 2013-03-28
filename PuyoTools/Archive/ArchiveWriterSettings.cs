using System;
using System.Drawing;
using System.Windows.Forms;

using PuyoTools.GUI;

namespace PuyoTools.Archive
{
    // This implimentation of the ArchiveWriterSettings class contains the panel content
    // for the archive creator.
    public abstract partial class ArchiveWriterSettings
    {
        public abstract void SetPanelContent(ArchiveCreator.SettingsPanel panel);
        public abstract void SetSettings();
    }

    // AFS Writer
    public partial class AfsWriterSettings : ArchiveWriterSettings
    {
        private ComboBox BlockSizeBox;
        private RadioButton[] VersionRadio;

        public override void SetPanelContent(ArchiveCreator.SettingsPanel panel)
        {
            panel.AddComboBox(out BlockSizeBox, "Block Size", new string[] { "2048", "16" }, ComboBoxStyle.DropDownList);
            panel.AddRadioButtons(out VersionRadio, "AFS Version", new string[] { "Version 1", "Version 2" });
        }

        public override void SetSettings()
        {
            BlockSize = int.Parse((string)BlockSizeBox.Items[BlockSizeBox.SelectedIndex]);
            Version = (VersionRadio[0].Checked ? AfsVersion.Version1 : AfsVersion.Version2);
        }
    }

    // GVM Writer
    public partial class GvmWriterSettings : ArchiveWriterSettings
    {
        private CheckBox FilenameCheckbox, GlobalIndexCheckbox, FormatsCheckbox, DimensionsCheckbox;

        public override void SetPanelContent(ArchiveCreator.SettingsPanel panel)
        {
            panel.AddCheckBox(out FilenameCheckbox, "Store Filenames");
            panel.AddCheckBox(out GlobalIndexCheckbox, "Store Global Indicies");
            panel.AddCheckBox(out FormatsCheckbox, "Store Texture Format");
            panel.AddCheckBox(out DimensionsCheckbox, "Store Texture Dimensions");
        }

        public override void SetSettings()
        {
            Filename = FilenameCheckbox.Checked;
            GlobalIndex = GlobalIndexCheckbox.Checked;
            Formats = FormatsCheckbox.Checked;
            Dimensions = DimensionsCheckbox.Checked;
        }
    }

    // SNT Writer
    public partial class SntWriterSettings : ArchiveWriterSettings
    {
        private RadioButton[] TypeRadio;

        public override void SetPanelContent(ArchiveCreator.SettingsPanel panel)
        {
            panel.AddRadioButtons(out TypeRadio, "SNT Type", new string[] { "PlayStation 2", "PlayStation Portable" });
        }

        public override void SetSettings()
        {
            Type = (TypeRadio[0].Checked ? SntType.Ps2 : SntType.Psp);
        }
    }
}