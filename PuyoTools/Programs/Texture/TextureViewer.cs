using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using Extensions;

namespace PuyoTools
{
    public class TextureViewer : Form
    {
        Panel imagePanel;
        PictureBox image = new PictureBox() {
            Location  = new Point(0, 0),
            Size      = new Size(0, 0),
            Image     = null,
            //BackColor = Color.White,
            BackgroundImage = BitmapResources.TextureViewerBackground,
            BackgroundImageLayout = ImageLayout.Tile,
        };
        StatusStrip statusStrip;
        ToolStripStatusLabel statusStripText;
        //ToolStripComboBox backColorSelect;
        string imageName = String.Empty;
        bool openedFromArchive;

        public TextureViewer()
        {
            openedFromArchive = false;
            CreateDisplay();
            this.ShowDialog();
        }

        // Loaded from the Archive Explorer
        public TextureViewer(Stream stream, string filename)
        {
            // Create display
            openedFromArchive = true;
            CreateDisplay();

            // Now load the image
            Bitmap bitmap;
            try
            {
                bitmap = LoadImage(stream, filename);
            }
            catch (TextureFormatNeedsPalette)
            {
                throw new TextureFormatNeedsPalette();
            }

            // Only bother if an image was actually produced
            if (bitmap != null && !bitmap.Size.IsEmpty)
            {
                DisplayImage(bitmap, filename);
                this.ShowDialog();
            }
            else
                this.Dispose();
        }

        // Loaded from the Archive Explorer
        public TextureViewer(Stream stream, string filename, Stream palette)
        {
            // Create display
            openedFromArchive = true;
            CreateDisplay();

            // Now load the image
            Bitmap bitmap = LoadImage(stream, filename, palette);

            // Only bother if an image was actually produced
            if (bitmap != null && !bitmap.Size.IsEmpty)
            {
                DisplayImage(bitmap, filename);
                this.ShowDialog();
            }
            else
                this.Dispose();
        }

        private void CreateDisplay()
        {
            // Set up the form
            FormContent.Create(this, "Texture Viewer", new Size(512, 256));

            // Set up the background color selection
            /*backColorSelect = new ToolStripComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            backColorSelect.Items.AddRange(new string[] { "White", "Black", "Red", "Green", "Blue" });
            backColorSelect.SelectedIndex = 0;
            backColorSelect.MaxDropDownItems = backColorSelect.Items.Count;
            backColorSelect.SelectedIndexChanged += new EventHandler(ChangeBackColor);*/

            // Set up the toolstrip
            ToolStrip toolStrip;

            if (openedFromArchive)
            {
                toolStrip = new ToolStrip(new ToolStripItem[] {
                    new ToolStripButton("Save as PNG", IconResources.Save, SaveImage),
                    new ToolStripSeparator(),
                    new ToolStripButton("Copy to Clipboard", null, CopyImageToClipboard),
                    //new ToolStripSeparator(),
                    //new ToolStripLabel("Background: "),
                    //backColorSelect,
                });
            }
            else
            {
                toolStrip = new ToolStrip(new ToolStripItem[] {
                    new ToolStripButton("Open", IconResources.Open, delegate(object sender, EventArgs e) { OpenImage(); }),
                    new ToolStripSeparator(),
                    new ToolStripButton("Save as PNG", IconResources.Save, SaveImage),
                    new ToolStripSeparator(),
                    new ToolStripButton("Copy to Clipboard", null, CopyImageToClipboard),
                    //new ToolStripSeparator(),
                    //new ToolStripLabel("Background: "),
                    //backColorSelect,
                });
            }

            this.Controls.Add(toolStrip);

            // Set up the status strip
            statusStrip = new StatusStrip()
            {
                SizingGrip = false,
            };
            statusStripText = new ToolStripStatusLabel()
            {
                Size = new Size(statusStrip.Size.Width, statusStrip.Size.Height),
            };

            statusStrip.Items.Add(statusStripText);
            this.Controls.Add(statusStrip);

            // Set up the image panel
            imagePanel = new Panel()
            {
                Location = new Point(0, 25),
                Size = new Size(this.Size.Width, 512),
                AutoScroll = true,
            };

            imagePanel.Controls.Add(image);
            this.Controls.Add(imagePanel);
        }

        private void OpenImage()
        {
            string file = FileSelectionDialog.OpenFile("Select Texture",
                "Supported Textures (*.cnx;*.gim;*.gmp;*.gvr;*.pvr;*.pvz;*.svr)|*.cnx;*.gim;*.gmp;*.gvr;*.pvr;*.pvz;*.svr|" +
                "CNX Compressed Texture (*.cnx)|*.cnx|" +
                "GIM Texture (*.gim)|*.gim|" +
                "GMP Texture (*.gmp)|*.gmp|" +
                "GVR Texture (*.gvr)|*.gvr|" +
                "PVR Texture (*.pvr;*.pvz)|*.pvr;*.pvz|" +
                "SVR Texture (*.svr)|*.svr");

            // Don't continue if a file wasn't selected
            if (file == null || file == string.Empty)
                return;

            Bitmap bitmap = LoadImage(file);
            DisplayImage(bitmap, file);
        }

        private Bitmap LoadImage(string filename)
        {
            try
            {
                // Open the file
                Textures imageClass;
                Stream file;
                using (file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    imageClass = new Textures(file, filename);

                    // Get the image
                    if (imageClass.Format == TextureFormat.NULL)
                    {
                        // Decompress the file and see if we can get anything
                        Compression compression = new Compression(file, filename);
                        if (compression.Format == CompressionFormat.NULL)
                            throw new TextureFormatNotSupported();
                        file = compression.Decompress();

                        imageClass = new Textures(file, filename);
                        
                        if (imageClass.Format == TextureFormat.NULL)
                            throw new TextureFormatNotSupported();
                    }

                    try
                    {
                        return imageClass.Decode();
                    }
                    catch (TextureFormatNeedsPalette)
                    {
                        // Load palette data and then unpack again
                        if (Path.GetDirectoryName(filename) != String.Empty)
                            imageClass.Decoder.PaletteData = LoadPaletteFile(Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + imageClass.PaletteFilename, imageClass);
                        else
                            imageClass.Decoder.PaletteData = LoadPaletteFile(imageClass.PaletteFilename, imageClass);

                        return imageClass.Decode();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private Bitmap LoadImage(Stream data, string filename)
        {
            try
            {
                // Get and return the image
                Textures imageClass = new Textures(data, filename);
                try
                {
                    return imageClass.Decode();
                }
                catch (TextureFormatNeedsPalette)
                {
                    throw new TextureFormatNeedsPalette();
                }
            }
            catch (TextureFormatNeedsPalette)
            {
                throw new TextureFormatNeedsPalette();
            }
            catch
            {
                return null;
            }
        }

        private Bitmap LoadImage(Stream data, string filename, Stream palette)
        {
            try
            {
                // Get and return the image
                Textures imageClass = new Textures(data, filename);
                if (imageClass.Format != TextureFormat.NULL)
                {
                    imageClass.Decoder.PaletteData = palette;
                    return imageClass.Decode();
                }

                throw new Exception();
            }
            catch
            {
                return null;
            }
        }

        private void DisplayImage(Bitmap bitmap, string filename)
        {
            // Display the image, if it is a valid image
            if (bitmap == null || bitmap.Size.IsEmpty)
            {
                image.Image = null;
                image.Size  = new Size(0, 0);

                // Update the status strip and filename
                imageName            = Path.GetFileName(filename);
                statusStripText.Text = String.Format("{0} could not be loaded.", (Path.GetFileName(filename) == String.Empty ? "Image" : '"' + Path.GetFileName(filename) + '"'));
            }
            else
            {
                image.Image = bitmap;
                image.Size  = new Size(bitmap.Size.Width, bitmap.Size.Height);

                // See if the image is bigger than the window
                if (bitmap.Size.Width > Screen.GetWorkingArea(this).Size.Width ||
                    bitmap.Size.Height + 25 + 22 > Screen.GetWorkingArea(this).Size.Height)
                {
                    // Yes it is!
                    this.ClientSize = new Size(Math.Max(Math.Min(Screen.GetWorkingArea(this).Size.Width, bitmap.Size.Width + SystemInformation.VerticalScrollBarWidth), 512), Math.Min(Screen.GetWorkingArea(this).Size.Height - 25 - 22, bitmap.Size.Height + SystemInformation.HorizontalScrollBarHeight));
                    this.CenterToScreen();
                    imagePanel.Size = new Size(Math.Min(bitmap.Size.Width + SystemInformation.VerticalScrollBarWidth, this.ClientSize.Width), Math.Min(bitmap.Size.Height + SystemInformation.HorizontalScrollBarHeight, this.ClientSize.Height - 25 - 22));

                    // Center image on screen
                    if (bitmap.Size.Width < 512)
                        imagePanel.Location = new Point((this.ClientSize.Width / 2) - (bitmap.Size.Width / 2), imagePanel.Location.Y);
                    else
                        imagePanel.Location = new Point(0, imagePanel.Location.Y);
                }
                else
                {
                    this.ClientSize = new Size(Math.Max(bitmap.Size.Width, 512), bitmap.Size.Height + 25 + 22);
                    this.CenterToScreen();
                    imagePanel.Size = new Size(bitmap.Size.Width, bitmap.Size.Height);

                    // Center image on screen
                    if (bitmap.Size.Width < 512)
                        imagePanel.Location = new Point((this.ClientSize.Width / 2) - (bitmap.Size.Width / 2), imagePanel.Location.Y);
                    else
                        imagePanel.Location = new Point(0, imagePanel.Location.Y);
                }

                // Update the status strip and filename
                imageName            = Path.GetFileName(filename);
                statusStripText.Text = String.Format("{0} successfully loaded! (Dimensions: {1} x {2})", (Path.GetFileName(filename) == String.Empty ? "Image" : '"' + Path.GetFileName(filename) + '"'), bitmap.Size.Width, bitmap.Size.Height);
            }
        }

        // Change background color
        /*private void ChangeBackColor(object sender, EventArgs e)
        {
            switch (((ToolStripComboBox)sender).SelectedIndex)
            {
                case 0: image.BackColor = Color.White; break;
                case 1: image.BackColor = Color.Black; break;
                case 2: image.BackColor = Color.Red;   break;
                case 3: image.BackColor = Color.Green; break;
                case 4: image.BackColor = Color.Blue;  break;
            }
        }*/

        // Save Image
        private void SaveImage(object sender, EventArgs e)
        {
            // Make sure we have an image
            if (image.Image == null || image.Image.Size.IsEmpty)
                return;

            string file = FileSelectionDialog.SaveFile("Save Image",
                Path.GetFileNameWithoutExtension(imageName),
                "PNG Image (*.png)|*.png|" +
                "All Files (*.*)|*.*");

            // Make sure we entered a filename
            if (file == null || file == String.Empty)
                return;

            // Now save the image
            try
            {
                using (FileStream output = new FileStream(file, FileMode.Create, FileAccess.Write))
                    image.Image.Save(output, ImageFormat.Png);
            }
            catch
            {
            }
        }

        // Load Palette File
        private Stream LoadPaletteFile(string filename, Textures imageClass)
        {
            // See if the palette file exists
            if (File.Exists(filename))
            {
                using (FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    return input.Copy();
            }

            return null;
        }

        // Copy Image to Clipboard
        private void CopyImageToClipboard(object sender, EventArgs e)
        {
            // Copy image to clipboard
            if (image.Image != null && !image.Image.Size.IsEmpty)
            {
                MemoryStream ms = new MemoryStream();
                image.Image.Save(ms, ImageFormat.Png);
                IDataObject dataObject = new DataObject();
                dataObject.SetData("PNG", false, ms);
                Clipboard.SetDataObject(dataObject, true);
            }
        }
    }
}