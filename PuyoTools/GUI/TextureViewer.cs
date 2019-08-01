using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

using PuyoTools.Modules.Texture;

namespace PuyoTools.GUI
{
    public partial class TextureViewer : Form
    {
        private Stream textureStream;

        public TextureViewer()
        {
            InitializeComponent();

            this.Icon = IconResources.ProgramIcon;
            this.ClientSize = new Size(512, this.ClientSize.Height);
            this.MinimumSize = this.Size;

            texturePanel.BackColor = Color.FromArgb(68, 68, 68);

            // Hide the texture information and texture display until a texture is opened
            textureInfoPanel.Visible = false;
            textureDisplay.Visible = false;

            // Disable menu entries until a texture is loaded
            saveToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
        }

        public void OpenTexture(Stream data, string fname, TextureFormat format)
        {
            Bitmap textureBitmap;
            Texture.Read(data, out textureBitmap, format);

            DisplayTexture(textureBitmap, fname, format);
        }

        public void OpenTexture(Stream data, string fname, Stream paletteData, TextureFormat format)
        {
            Bitmap textureBitmap;

            TextureBase texture = Texture.GetModule(format);
            texture.PaletteStream = paletteData;
            texture.Read(data, out textureBitmap);

            DisplayTexture(textureBitmap, fname, format);
        }

        private void DisplayTexture(Bitmap textureBitmap, string fname, TextureFormat format)
        {
            textureDisplay.Image = textureBitmap;

            // Adjust the textureDisplay and center it
            textureDisplay.Size = textureBitmap.Size;

            // Now let's adjust the width and height of the form
            this.ClientSize = new Size(Math.Max(Math.Min(1024, textureDisplay.Width), texturePanel.Width), this.ClientSize.Height - texturePanel.Height + Math.Max(Math.Min(512, textureDisplay.Height), texturePanel.Height));


            texturePanel_ClientSizeChanged(null, null);

            this.CenterToScreen();

            // Display information about the texture
            textureNameLabel.Text = (fname == String.Empty ? "Unnamed" : fname);
            textureDimensionsLabel.Text = textureBitmap.Width + " x " + textureBitmap.Height;
            textureFormatLabel.Text = Texture.GetModule(format).Name;

            textureInfoPanel.Visible = true;
            textureDisplay.Visible = true;

            saveToolStripMenuItem.Enabled = true;
            copyToolStripMenuItem.Enabled = true;
        }

        private void texturePanel_ClientSizeChanged(object sender, EventArgs e)
        {
            if (textureDisplay.Width > texturePanel.ClientSize.Width)
                textureDisplay.Left = 0;
            else
                textureDisplay.Left = (texturePanel.ClientSize.Width / 2) - (textureDisplay.Width / 2);

            if (textureDisplay.Height > texturePanel.ClientSize.Height)
                textureDisplay.Top = 0;
            else
                textureDisplay.Top = (texturePanel.ClientSize.Height / 2) - (textureDisplay.Height / 2);

            texturePanel.Refresh();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "All Files (*.*)|*.*";
            ofd.Title = "Open Texture";

            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (textureStream != null)
                {
                    textureStream.Close();
                }

                textureStream = File.OpenRead(ofd.FileName);

                // Let's determine first if it is a texture
                TextureFormat textureFormat;

                textureFormat = Texture.GetFormat(textureStream, ofd.SafeFileName);

                if (textureFormat == TextureFormat.Unknown)
                {
                    // It's not a texture. Maybe it's compressed?
                    CompressionFormat compressionFormat = Compression.GetFormat(textureStream, ofd.SafeFileName);
                    if (compressionFormat != CompressionFormat.Unknown)
                    {
                        // The file is compressed! Let's decompress it and then try to determine if it is a texture
                        MemoryStream decompressedData = new MemoryStream();
                        Compression.Decompress(textureStream, decompressedData, compressionFormat);
                        textureStream.Close();
                        decompressedData.Position = 0;

                        // Now with this decompressed data, let's determine if it is a texture
                        textureFormat = Texture.GetFormat(decompressedData, ofd.SafeFileName);

                        if (textureFormat != TextureFormat.Unknown)
                        {
                            // It appears to be a texture. Set data to the decompressed data
                            textureStream = decompressedData;
                        }
                        else
                        {
                            // Hmm... still doesn't appear to be a texture. Just ignore this file then.
                            return;
                        }
                    }
                    else
                    {
                        // Hmm... doesn't appear to be a texture. Just ignore this file then.
                        textureStream.Close();
                        return;
                    }
                }

                // This is a texture. Let's open it.
                try
                {
                    OpenTexture(textureStream, ofd.SafeFileName, textureFormat);
                }
                catch (TextureNeedsPaletteException)
                {
                    // Seems like we need a palette for this texture. Let's try to find one.
                    string paletteName = Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName)) + Texture.Formats[textureFormat].PaletteFileExtension;

                    if (File.Exists(paletteName))
                    {
                        // Looks like the palette file exists. Let's load that with the texture
                        textureStream.Position = 0;
                        using (FileStream paletteData = File.OpenRead(paletteName))
                        {
                            OpenTexture(textureStream, ofd.SafeFileName, paletteData, textureFormat);
                        }
                    }
                    else
                    {
                        // Can't load the palette, so close the texture.
                        textureStream.Close();
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ctrl + C should preform this as well, so there is no need to override ProcessCmdKey.
            MemoryStream ms = new MemoryStream();
            textureDisplay.Image.Save(ms, ImageFormat.Png);
            IDataObject dataObject = new DataObject();
            dataObject.SetData("PNG", false, ms);
            Clipboard.SetDataObject(dataObject, true);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileNameWithoutExtension(textureNameLabel.Text) + ".png";
            sfd.Filter = "PNG Image (*.png)|*.png";
            sfd.Title = "Save Texture";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (FileStream outStream = File.Create(sfd.FileName))
                {
                    textureDisplay.Image.Save(outStream, ImageFormat.Png);
                }
            }
        }

        private void lightBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (darkBackgroundToolStripMenuItem.Checked)
            {
                darkBackgroundToolStripMenuItem.Checked = false;
                lightBackgroundToolStripMenuItem.Checked = true;

                textureDisplay.BackgroundImage = BitmapResources.CheckeredBackgroundLight;
            }
        }

        private void darkBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lightBackgroundToolStripMenuItem.Checked)
            {
                lightBackgroundToolStripMenuItem.Checked = false;
                darkBackgroundToolStripMenuItem.Checked = true;

                textureDisplay.BackgroundImage = BitmapResources.CheckeredBackgroundDark;
            }
        }
    }
}
