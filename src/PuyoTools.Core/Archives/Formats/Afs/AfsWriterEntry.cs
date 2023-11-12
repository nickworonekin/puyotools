using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Afs
{
    public class AfsWriterEntry : ArchiveWriterEntry
    {
        public AfsWriterEntry(Stream source, string? name = null, bool leaveOpen = false) : base(source, name, leaveOpen)
        {
            if (source is FileStream fs)
            {
                Timestamp = File.GetLastWriteTime(fs.Name);
            }
        }

        public DateTime? Timestamp { get; }
    }
}
