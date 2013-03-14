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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Initalize();

            Application.Run(new GUI.MainWindow());
        }

        // Initalize directories
        private static void Initalize()
        {
            Old.Compression.InitalizeDictionary();
            Old.Archive.InitalizeDictionary();
            Old.Textures.InitalizeDictionary();

            Compression.PTCompression.Initalize();
            Archive.PTArchive.Initalize();
            Texture.PTTexture.Initalize();
        }
    }
}