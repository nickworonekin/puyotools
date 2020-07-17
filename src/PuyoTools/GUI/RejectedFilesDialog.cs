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
    public partial class RejectedFilesDialog : Form
    {
        public RejectedFilesDialog(List<string> files)
        {
            InitializeComponent();

            this.Icon = IconResources.ProgramIcon;
            this.MinimumSize = this.Size;

            listView_ClientSizeChanged(null, null);

            // Make the list view rows bigger
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(1, 20);
            listView.SmallImageList = imageList;

            // Populate the list view.
            foreach (string file in files)
            {
                listView.Items.Add(file);
            }
        }

        private void listView_ClientSizeChanged(object sender, EventArgs e)
        {
            listView.Columns[0].Width = listView.ClientSize.Width - 20;
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
