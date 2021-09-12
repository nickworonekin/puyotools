using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.GUI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            Icon = IconResources.ProgramIcon;
            MinimumSize = Size;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutPuyoTools().Show();
        }

        private void compressionButton_Click(object sender, EventArgs e)
        {
            compressionMenuStrip.Show(MousePosition);
        }

        private void archivesButton_Click(object sender, EventArgs e)
        {
            archivesMenuStrip.Show(MousePosition);
        }

        private void texturesButton_Click(object sender, EventArgs e)
        {
            texturesMenuStrip.Show(MousePosition);
        }

        private void compressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FileCompressor().Show();
        }

        private void decompressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FileDecompressor().Show();
        }

        private void createToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new ArchiveCreator().Show();
        }

        private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new ArchiveExtractor().Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new ArchiveExplorer().Show();
        }

        private void decodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new TextureDecoder().Show();
        }

        private void encodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new TextureEncoder().Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            new TextureViewer().Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
