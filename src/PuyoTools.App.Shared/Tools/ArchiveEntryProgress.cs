using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.App.Tools
{
    class ArchiveEntryProgress : ToolProgress
    {
        public ArchiveEntryProgress(double progress, string file) : base(progress, file)
        {
        }
    }
}
