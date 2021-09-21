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
        /// Gets the zero-based index of the file currently being processed by the tool.
        /// </summary>
        public int Index { get; }

        public ToolProgress(string file, int index)
        {
            File = file;
            Index = index;
        }
    }
}
