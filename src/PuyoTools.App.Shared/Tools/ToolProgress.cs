using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.App.Tools
{
    class ToolProgress
    {
        /// <summary>
        /// Gets the name of the file currently being processed by the tool.
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Gets the progress of the tool (0 to 1).
        /// </summary>
        public double Progress { get; }

        public ToolProgress(double progress, string file)
        {
            Progress = progress;
            File = file;
        }
    }
}
