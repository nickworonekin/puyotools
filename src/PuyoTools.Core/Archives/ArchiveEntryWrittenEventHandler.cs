using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Archive
{
    /// <summary>
    /// Represents a method that will handle the archive entry written event.
    /// </summary>
    public delegate void ArchiveEntryWrittenEventHandler(object sender, ArchiveEntryWrittenEventArgs e);
}
