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
        public const string Version = "2.0.1";

        [ImportMany("IPuyoToolsPlugin")]
        static IEnumerable<IPuyoToolsPlugin> Plugins;

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
            Compression.Initalize();
            Archive.Initalize();
            Texture.Initalize();

            // Load plugins
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

                    if (plugin is Modules.Archive.ArchiveBase)
                    {
                        // Archive module
                        Modules.Archive.ArchiveBase module = (Modules.Archive.ArchiveBase)plugin;
                        Archive.Formats.Add(ArchiveFormat.Plugin + archivePlugins, module);
                        archivePlugins++;
                    }

                    else if (plugin is Modules.Compression.CompressionBase)
                    {
                        // Compression module
                        Modules.Compression.CompressionBase module = (Modules.Compression.CompressionBase)plugin;
                        Compression.Formats.Add(CompressionFormat.Plugin + compressionPlugins, module);
                        compressionPlugins++;
                    }

                    else if (plugin is Modules.Texture.TextureBase)
                    {
                        // Texture module
                        Modules.Texture.TextureBase module = (Modules.Texture.TextureBase)plugin;
                        Texture.Formats.Add(TextureFormat.Plugin + texturePlugins, module);
                        texturePlugins++;
                    }
                }
            }
            catch { }
        }
    }
}