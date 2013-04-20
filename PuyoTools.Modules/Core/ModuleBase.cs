using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PuyoTools.Modules
{
    public abstract class ModuleBase
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// Determine if the data in source is in this specified format.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="fname">Filename</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public abstract bool Is(Stream source, int length, string fname);

        /// <summary>
        /// Returns a new writer settings object for this format.
        /// </summary>
        /// <returns>A new ModuleWriterSettings object.</returns>
        public virtual ModuleWriterSettings WriterSettingsObject()
        {
            return null;
        }
    }

    public abstract class ModuleWriterSettings
    {
        /// <summary>
        /// Set panel content.
        /// </summary>
        /// <param name="panel">The panel to add the controls to.</param>
        public abstract void SetPanelContent(Panel panel);

        /// <summary>
        /// Set the settings from the controls in the panel.
        /// </summary>
        public abstract void SetSettings();

        /// <summary>
        /// Parse command line argumentes for this format.
        /// </summary>
        /// <param name="args">The arguments.</param>
        //public abstract void ParseCommandLineArguments(Dictionary<string, string> args);
    }
}