using PuyoTools.GUI;
using PuyoTools.Modules.Archive;
using PuyoTools.Modules.Compression;
using PuyoTools.Modules.Texture;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows.Forms;

namespace PuyoTools
{
    public static class PuyoTools
    {
        public const string Version = "2.0.4";

        [ImportMany("IPuyoToolsPlugin")]
        static IEnumerable<IPuyoToolsPlugin> Plugins;

        [STAThread]
        public static void Main()
        {
            LoadPlugins();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }

        // Load plugins
        private static void LoadPlugins()
        {
            int archivePlugins = 0, compressionPlugins = 0, texturePlugins = 0;
            try
            {
                // Get all of the DLLs in the Plugins directory and load them if they contain any classes that impliment IPuyoToolsPlugin.
                AggregateCatalog catalog = new AggregateCatalog(new DirectoryCatalog("Plugins", "*.dll"), new AssemblyCatalog(Assembly.GetExecutingAssembly()));
                CompositionContainer container = new CompositionContainer(catalog);
                Plugins = container.GetExportedValues<IPuyoToolsPlugin>();

                foreach (IPuyoToolsPlugin plugin in Plugins)
                {
                    // Now, let's attempt to determine which type of plugin this is
                    // and add it to the appropiate module lists.

                    if (plugin is ArchiveBase archiveModule)
                    {
                        // Archive module
                        Archive.Formats.Add(ArchiveFormat.Plugin + archivePlugins, archiveModule);
                        archivePlugins++;
                    }

                    else if (plugin is CompressionBase compressionModule)
                    {
                        // Compression module
                        Compression.Formats.Add(CompressionFormat.Plugin + compressionPlugins, compressionModule);
                        compressionPlugins++;
                    }

                    else if (plugin is TextureBase textureModule)
                    {
                        // Texture module
                        Texture.Formats.Add(TextureFormat.Plugin + texturePlugins, textureModule);
                        texturePlugins++;
                    }
                }
            }
            catch { }
        }
    }
}