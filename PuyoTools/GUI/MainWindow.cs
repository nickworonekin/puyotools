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
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Icon = IconResources.ProgramIcon;
            this.MinimumSize = this.Size;

            // Write out the version, but do it in a hackish way
            int oldWidth = versionLabel.Width;
            versionLabel.Text = "Version " + PuyoTools.Version;
            versionLabel.Left -= (versionLabel.Width - oldWidth);
        }

        private void selectFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new Old.Decompressor();
            (new FileDecompressor()).Show();
        }

        private void selectDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new Old.Decompressor(true);
        }

        private void selectFilesToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //new Old.Compressor();
            (new FileCompressor()).Show();
        }

        private void selectDirectoryToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //new Old.Compressor(true);
            (new FileCompressor()).Show();
        }

        private void selectFilesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //new Old.ArchiveExtractor();
            (new ArchiveExtractor()).Show();
        }

        private void selectDirectoryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //new Old.ArchiveExtractor(true);
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new Old.ArchiveCreator();
            (new ArchiveCreator()).Show();
        }

        private void explorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new ArchiveExplorer()).Show();
        }

        private void selectFilesToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //new Old.TextureDecoder();
        }

        private void selectDirectoryToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //new Old.TextureDecoder(true);
        }

        private void selectFilesToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //new Old.TextureEncoder();
        }

        private void selectDirectoryToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //new Old.TextureEncoder(true);
        }

        private void viewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new TextureViewer()).Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/nickworonekin/puyotools");
        }

        private void decompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new FileDecompressor()).Show();
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new ArchiveExtractor()).Show();
        }

        private void decodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new TextureDecoder()).Show();
        }

        private void compressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new FileCompressor()).Show();
        }

        private void encodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new TextureEncoder()).Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutPuyoTools()).Show();
        }
    }
}
