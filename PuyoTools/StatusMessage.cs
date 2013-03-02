using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PuyoTools
{
    public class StatusMessage : Form
    {
        private Label
            displayFileLabel = new Label();

        private ProgressBar
            progress      = new ProgressBar(),
            progressLocal = new ProgressBar();

        private int
            currentFile,
            currentFileLocal,
            totalFiles,
            totalFilesLocal;

        List<string>
            fileList = new List<string>();

        // Set up our form
        public StatusMessage(string title, string[] files)
        {
            // Ok, first let's create the file list
            foreach (string i in files)
                fileList.Add(i);

            // Now, get our total files
            totalFiles = files.Length;

            // Now, let's set up our form
            FormContent.Create(this, title, new Size(300, 96), false);

            // Add Processing
            FormContent.Add(this, new Label(),
                "Processing",
                new Point(0, 8),
                new Size(this.Width, 16),
                ContentAlignment.TopCenter,
                new Font(SystemFonts.DialogFont.FontFamily.Name, SystemFonts.DialogFont.Size, FontStyle.Bold));

            // File Number
            FormContent.Add(this, displayFileLabel,
                String.Empty,
                new Point(0, 24),
                new Size(this.Width, 48),
                ContentAlignment.TopCenter);

            // Add the progress bar
            FormContent.Add(this, progress,
                new Point(8, 72),
                new Size(this.Width - 22, 16),
                totalFiles);

            // Ok, set the current file now
            CurrentFile = 0;
        }

        public void addProgressBarLocal()
        {
            // First, expand the status box
            this.Size = new Size(this.Size.Width, this.Size.Height + 16);

            // Next, move down the current progress bar
            progress.Location = new Point(progress.Location.X, progress.Location.Y + 16);

            // Now, add the local progress bar, for archives
            FormContent.Add(this, progressLocal,
                new Point(8, 72),
                new Size(this.Width - 22, 12),
                0);
        }

        // Set the current file label
        public int CurrentFile
        {
            set
            {
                // Update the current value
                currentFile = value;

                // Update the label
                displayFileLabel.Text = String.Format(
                    "File {0} of {1}\n\n{2}",
                        (currentFile + 1).ToString("#,0"),
                        totalFiles.ToString("#,0"),
                        Path.GetFileName(fileList[currentFile]));

                // Update the progress bar
                progress.Value = currentFile;
            }
        }

        // Set the local current file
        public int CurrentFileLocal
        {
            set
            {
                // Update the local file
                currentFileLocal = value;
                progressLocal.Value = currentFileLocal;
            }
        }

        // Set the total files for the archive
        public int TotalFilesLocal
        {
            set
            {
                totalFilesLocal = value;
                progressLocal.Maximum = totalFilesLocal;
            }
        }

        // Add file
        public void AddFile(string file)
        {
            fileList.Add(file);
            totalFiles++;
            progress.Maximum = totalFiles;
        }
    }
}