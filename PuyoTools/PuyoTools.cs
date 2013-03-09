using System;
using System.Windows.Forms;

namespace PuyoTools
{
    public static class PuyoTools
    {
        public const string Version = "1.1.0";

        [STAThread]
        public static void Main()
        {
            Initalize();
            Application.EnableVisualStyles();
            Application.Run(new GUI.MainWindow());
        }

        // Initalize directories
        private static void Initalize()
        {
            Compression.InitalizeDictionary();
            Archive.InitalizeDictionary();
            Textures.InitalizeDictionary();

            PuyoTools2.Compression.Compression.Initalize();
            PuyoTools2.Archive.Archive.Initalize();
        }
    }
}