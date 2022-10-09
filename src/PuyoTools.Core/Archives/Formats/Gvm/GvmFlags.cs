using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Gvm
{
    [Flags]
    internal enum GvmFlags : ushort
    {
        /// <summary>
        /// Specifies global indexes are provided.
        /// </summary>
        GlobalIndexes = (1 << 0),

        /// <summary>
        /// Specifies texture dimensions are provided within the entry table.
        /// </summary>
        Dimensions = (1 << 1),

        /// <summary>
        /// Specifies pixel and data formats are provided within the entry table.
        /// </summary>
        Formats = (1 << 2),

        /// <summary>
        /// Specifies filenames are present within the entry table.
        /// </summary>
        Filenames = (1 << 3),
    }
}
