using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PuyoTools.GUI
{
    public partial class ToolForm : Form
    {
        protected List<string> fileList;

        public ToolForm()
        {
            InitializeComponent();

            this.Icon = IconResources.ProgramIcon;
            this.MinimumSize = this.Size;

            fileList = new List<string>();
        }

        private void addFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";
            ofd.Multiselect = true;
            ofd.Title = "Add Files";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileList.AddRange(ofd.FileNames);

                if (fileList.Count > 0)
                    runButton.Enabled = true;
            }
        }

        private void addDirectoryButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select a directory.";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("Include files within sub-directories?", "Subdirectories", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    fileList.AddRange(Directory.GetFiles(fbd.SelectedPath, "*.*", SearchOption.AllDirectories));
                }
                else
                {
                    fileList.AddRange(Directory.GetFiles(fbd.SelectedPath));
                }

                if (fileList.Count > 0)
                    runButton.Enabled = true;
            }
        }
    }
}
