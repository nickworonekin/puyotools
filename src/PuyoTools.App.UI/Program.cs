using PuyoTools.GUI;
using PuyoTools.Core.Archives;
using PuyoTools.Core.Compression;
using PuyoTools.Core.Textures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace PuyoTools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}