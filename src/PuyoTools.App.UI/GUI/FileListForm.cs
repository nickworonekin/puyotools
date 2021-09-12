using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.GUI
{
    public partial class FileListForm : Form
    {
        List<string> fileList;

        public FileListForm(List<string> files)
        {
            InitializeComponent();

            this.Icon = IconResources.ProgramIcon;
            this.MinimumSize = this.Size;

            listView_ClientSizeChanged(null, null);

            // Make the list view rows bigger
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(1, 20);
            listView.SmallImageList = imageList;

            // Add the files to the file list, and populate the list view.
            fileList = files;

            foreach (string file in fileList)
            {
                listView.Items.Add(file);
            }
        }

        private void listView_ClientSizeChanged(object sender, EventArgs e)
        {
            listView.Columns[0].Width = listView.ClientSize.Width - 20;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We're going to assume that the selected indicies are sorted in order.
            ListView.SelectedIndexCollection indicies = listView.SelectedIndices;

            for (int i = indicies.Count - 1; i >= 0; i--)
            {
                fileList.RemoveAt(indicies[i]);
                listView.Items.RemoveAt(indicies[i]);
            }
        }
    }
}
