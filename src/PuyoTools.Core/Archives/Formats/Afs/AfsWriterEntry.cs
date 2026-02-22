using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuyoTools.Core;

namespace PuyoTools.Archives.Formats.Afs
{
    public class AfsWriterEntry : ArchiveWriterEntry
    {
        public AfsWriterEntry(Stream source, string? name = null, bool leaveOpen = false) : base(source, name, leaveOpen)
        {
            if (source is FileStream fs)
            {
                Timestamp = File.GetLastWriteTime(fs.SafeFileHandle);
            }
            else if (source is LazyFileReadStream lfs)
            {
                Timestamp = File.GetLastWriteTime(lfs.Name);
            }
        }

        public DateTime? Timestamp { get; }
    }
}
