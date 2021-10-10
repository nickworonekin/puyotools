using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Archive
{
    /// <summary>
    /// Represents a method that will handle the archive entry writing event.
    /// </summary>
    public delegate void ArchiveEntryWritingEventHandler(object sender, ArchiveEntryWritingEventArgs e);
}
