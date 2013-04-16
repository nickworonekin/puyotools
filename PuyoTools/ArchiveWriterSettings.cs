using System;
using System.Drawing;
using System.Windows.Forms;

using PuyoTools.Modules.Archive;

namespace PuyoTools.GUI
{
    // This implimentation of the ArchiveWriterSettings class contains the panel content
    // for the archive creator.
    public abstract partial class ArchiveWriterSettingsGUI
    {
        public abstract void SetPanelContent(ArchiveCreator.SettingsPanel panel);
        public abstract void SetSettings(ArchiveWriterSettings settings);
    }

    // AFS Writer
    public class AfsWriterSettingsGUI : ArchiveWriterSettingsGUI
    {
        private ComboBox BlockSizeBox;
        private RadioButton[] VersionRadio;

        public override void SetPanelContent(ArchiveCreator.SettingsPanel panel)
        {
            panel.AddComboBox(out BlockSizeBox, "Block Size", new string[] { "2048", "16" }, ComboBoxStyle.DropDownList);
            panel.AddRadioButtons(out VersionRadio, "AFS Version", new string[] { "Version 1", "Version 2" });
        }

        public override void SetSettings(ArchiveWriterSettings settings)
        {
            AfsArchive.WriterSettings writerSettings = (AfsArchive.WriterSettings)settings;

            writerSettings.BlockSize = int.Parse((string)BlockSizeBox.Items[BlockSizeBox.SelectedIndex]);
            writerSettings.Version = (VersionRadio[0].Checked ? AfsArchive.WriterSettings.AfsVersion.Version1 : AfsArchive.WriterSettings.AfsVersion.Version2);
        }
    }

    // GVM Writer
    public class GvmWriterSettingsGUI : ArchiveWriterSettingsGUI
    {
        private CheckBox FilenameCheckbox, GlobalIndexCheckbox, FormatsCheckbox, DimensionsCheckbox;

        public override void SetPanelContent(ArchiveCreator.SettingsPanel panel)
        {
            panel.AddCheckBox(out FilenameCheckbox, "Store Filenames");
            panel.AddCheckBox(out GlobalIndexCheckbox, "Store Global Indicies");
            panel.AddCheckBox(out FormatsCheckbox, "Store Texture Format");
            panel.AddCheckBox(out DimensionsCheckbox, "Store Texture Dimensions");
        }

        public override void SetSettings(ArchiveWriterSettings settings)
        {
            GvmArchive.WriterSettings writerSettings = (GvmArchive.WriterSettings)settings;

            writerSettings.Filename = FilenameCheckbox.Checked;
            writerSettings.GlobalIndex = GlobalIndexCheckbox.Checked;
            writerSettings.Formats = FormatsCheckbox.Checked;
            writerSettings.Dimensions = DimensionsCheckbox.Checked;
        }
    }

    // PVM Writer
    public class PvmWriterSettingsGUI : ArchiveWriterSettingsGUI
    {
        private CheckBox FilenameCheckbox, GlobalIndexCheckbox, FormatsCheckbox, DimensionsCheckbox;

        public override void SetPanelContent(ArchiveCreator.SettingsPanel panel)
        {
            panel.AddCheckBox(out FilenameCheckbox, "Store Filenames");
            panel.AddCheckBox(out GlobalIndexCheckbox, "Store Global Indicies");
            panel.AddCheckBox(out FormatsCheckbox, "Store Texture Format");
            panel.AddCheckBox(out DimensionsCheckbox, "Store Texture Dimensions");
        }

        public override void SetSettings(ArchiveWriterSettings settings)
        {
            PvmArchive.WriterSettings writerSettings = (PvmArchive.WriterSettings)settings;

            writerSettings.Filename = FilenameCheckbox.Checked;
            writerSettings.GlobalIndex = GlobalIndexCheckbox.Checked;
            writerSettings.Formats = FormatsCheckbox.Checked;
            writerSettings.Dimensions = DimensionsCheckbox.Checked;
        }
    }

    // SNT Writer
    public class SntWriterSettingsGUI : ArchiveWriterSettingsGUI
    {
        private RadioButton[] TypeRadio;

        public override void SetPanelContent(ArchiveCreator.SettingsPanel panel)
        {
            panel.AddRadioButtons(out TypeRadio, "SNT Type", new string[] { "PlayStation 2", "PlayStation Portable" });
        }

        public override void SetSettings(ArchiveWriterSettings settings)
        {
            SntArchive.WriterSettings writerSettings = (SntArchive.WriterSettings)settings;

            writerSettings.Type = (TypeRadio[0].Checked ? SntArchive.WriterSettings.SntType.Ps2 : SntArchive.WriterSettings.SntType.Psp);
        }
    }
}