using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PuyoTools
{
    public class ArchivePackSettings
    {
        public int[] BlockSizes       { get; protected set; }
        public List<Control> Settings { get; protected set; }

        // ACX
        public class ACX : ArchivePackSettings
        {
            public ACX()
            {
                BlockSizes = new int[] { 4, 2048 };
                Settings   = new List<Control>();

                // Store filenames
                Settings.Add(new CheckBox() {
                    Text = "Store Filenames",
                    Checked = false,
                });
            }
        }

        // AFS
        public class AFS : ArchivePackSettings
        {
            public AFS()
            {
                BlockSizes = new int[] { 2048, 32 };
                Settings = new List<Control>();

                // AFS version
                Settings.Add(new Panel());
                Settings[Settings.Count - 1].Controls.AddRange(new Control[] {
                    new RadioButton() {
                        Text    = "AFS v1 (Dreamcast)",
                        Checked = false,
                    },
                    new RadioButton() {
                        Text    = "AFS v2 (PS2/GC/Xbox and After)",
                        Checked = true,
                    },
                });

                // Add filenames
                Settings.Add(new CheckBox() {
                    Text    = "Store Filenames",
                    Checked = true,
                });

                // Store creation times
                Settings.Add(new CheckBox() {
                    Text    = "Store Creation Times",
                    Checked = true,
                });
            }
        }

        // GNT
        public class GNT : ArchivePackSettings
        {
            public GNT()
            {
                BlockSizes = new int[] { 8, -1 };
                Settings   = new List<Control>();

                // Store filenames
                Settings.Add(new CheckBox() {
                    Text    = "Store Filenames",
                    Checked = false,
                });
            }
        }

        // GVM
        public class GVM : ArchivePackSettings
        {
            public GVM()
            {
                BlockSizes = new int[] { 16, -1 };
                Settings   = new List<Control>();

                // Store filenames
                Settings.Add(new CheckBox() {
                    Text    = "Store Filenames",
                    Checked = true,
                });

                // Store GVR's Global Index
                Settings.Add(new CheckBox() {
                    Text    = "Store GVR's Global Index",
                    Checked = true,
                });

                // Store GVR's Texture Dimensions
                Settings.Add(new CheckBox() {
                    Text    = "Store GVR's Texture Dimensions",
                    Checked = true,
                });

                // Store GVR's Pixel & Data Format
                Settings.Add(new CheckBox() {
                    Text    = "Store GVR's Pixel and Data Format",
                    Checked = true,
                });
            }
        }

        // MDL
        public class MDL : ArchivePackSettings
        {
            public MDL()
            {
                BlockSizes = new int[] { 4096 };
                Settings   = new List<Control>();

                // Store filenames
                Settings.Add(new CheckBox() {
                    Text    = "Store Filenames",
                    Checked = false,
                });
            }
        }

        // MRG
        public class MRG : ArchivePackSettings
        {
            public MRG()
            {
                BlockSizes = new int[] { 16, -1 };
                Settings   = new List<Control>();
            }
        }

        // NARC
        public class NARC : ArchivePackSettings
        {
            public NARC()
            {
                BlockSizes = new int[] { 4, -1 };
                Settings   = new List<Control>();

                // Store filenames
                Settings.Add(new CheckBox() {
                    Text    = "Store Filenames",
                    Checked = true,
                });
            }
        }

        // Unleashed ONE
        public class ONE : ArchivePackSettings
        {
            public ONE()
            {
                BlockSizes = new int[] { 32 };
                Settings   = new List<Control>();
            }
        }

        // PVM
        public class PVM : ArchivePackSettings
        {
            public PVM()
            {
                BlockSizes = new int[] { 16, -1 };
                Settings   = new List<Control>();

                // Store filenames
                Settings.Add(new CheckBox() {
                    Text    = "Store Filenames",
                    Checked = true,
                });

                // Store PVR's Global Index
                Settings.Add(new CheckBox() {
                    Text    = "Store PVR's Global Index",
                    Checked = true,
                });

                // Store PVR's Texture Dimensions
                Settings.Add(new CheckBox() {
                    Text    = "Store PVR's Texture Dimensions",
                    Checked = true,
                });

                // Store PVR's Pixel & Data Format
                Settings.Add(new CheckBox() {
                    Text    = "Store PVR's Pixel and Data Format",
                    Checked = true,
                });
            }
        }

        // Storybook
        public class SBA : ArchivePackSettings
        {
            public SBA()
            {
            }
        }

        // SNT
        public class SNT : ArchivePackSettings
        {
            public SNT()
            {
                BlockSizes = new int[] { 8, -1 };
                Settings   = new List<Control>();

                // SNT type
                Settings.Add(new Panel());
                Settings[Settings.Count - 1].Controls.AddRange(new Control[] {
                    new RadioButton() {
                        Text    = "PS2 SNT Archive",
                        Checked = true,
                    },
                    new RadioButton() {
                        Text    = "PSP SNT Archive",
                        Checked = false,
                    },
                });

                // Store filenames
                Settings.Add(new CheckBox() {
                    Text    = "Store Filenames",
                    Checked = false,
                });
            }
        }

        // SPK
        public class SPK : ArchivePackSettings
        {
            public SPK()
            {
                BlockSizes = new int[] { 16, -1 };
                Settings   = new List<Control>();
            }
        }

        // Fever 2 TEX
        public class TEX : ArchivePackSettings
        {
            public TEX()
            {
                BlockSizes = new int[] { 16, -1 };
                Settings   = new List<Control>();
            }
        }

        // Storybook TXD
        public class TXAG : ArchivePackSettings
        {
            public TXAG()
            {
                BlockSizes = new int[] { 64 };
                Settings   = new List<Control>();
            }
        }

        // VDD
        public class VDD : ArchivePackSettings
        {
            public VDD()
            {
                BlockSizes = new int[] { 2048, -1 };
                Settings   = new List<Control>();
            }
        }
    }
}