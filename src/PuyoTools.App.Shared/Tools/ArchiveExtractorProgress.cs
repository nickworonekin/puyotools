using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.App.Tools
{
    class ArchiveExtractorProgress : ToolProgress
    {
        /// <summary>
        /// Gets the name of the entry currently being extracted by the tool.
        /// </summary>
        public string Entry { get; }

        public ArchiveExtractorProgress(double progress, string file, string entry = null) : base(progress, file)
        {
            Entry = entry;
        }
    }
}
