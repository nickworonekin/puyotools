using System;
using System.Windows.Forms;

namespace PuyoTools
{
    public class FileSelectionDialog
    {
        // Open File Selection Dialog
        public static string OpenFile(string title, string filter)
        {
            OpenFileDialog ofd = new OpenFileDialog() {
                Multiselect      = false,
                RestoreDirectory = true,
                CheckFileExists  = true,
                CheckPathExists  = true,
                AddExtension     = true,
                Filter           = filter,
                DefaultExt       = string.Empty,
                Title            = title,
            };
            DialogResult result = ofd.ShowDialog();

            if (result != DialogResult.OK)
                return null;

            return ofd.FileName;
        }

        public static string[] OpenFiles(string title, string filter)
        {
            OpenFileDialog ofd = new OpenFileDialog() {
                Multiselect      = true,
                RestoreDirectory = true,
                CheckFileExists  = true,
                CheckPathExists  = true,
                AddExtension     = true,
                Filter           = filter,
                DefaultExt       = string.Empty,
                Title            = title,
            };
            DialogResult result = ofd.ShowDialog();

            if (result != DialogResult.OK)
                return null;

            return ofd.FileNames;
        }

        // Save File Selection Dialog
        public static string SaveFile(string title, string filename, string filter)
        {
            SaveFileDialog sfd   = new SaveFileDialog() {
                AddExtension     = true,
                DefaultExt       = string.Empty,
                FileName         = filename,
                Filter           = filter,
                OverwritePrompt  = true,
                RestoreDirectory = true,
                Title            = title,
                ValidateNames    = true,
            };
            DialogResult result = sfd.ShowDialog();
            if (result != DialogResult.OK)
                return null;

            return sfd.FileName;
        }

        // Save Directory Selection Dialog
        public static string SaveDirectory(string description)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description         = description;
            fbd.SelectedPath        = Application.StartupPath;
            fbd.ShowNewFolderButton = true;
            DialogResult result = fbd.ShowDialog();

            if (result != DialogResult.OK)
                return null;

            return fbd.SelectedPath;
        }
    }
}