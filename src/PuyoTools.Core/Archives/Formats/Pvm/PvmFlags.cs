using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Pvm
{
    [Flags]
    internal enum PvmFlags : ushort
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

        /// <summary>
        /// Specifies an MDLN chunk is present, which contains the filenames of the models associated with this PVM.
        /// </summary>
        MdlnChunk = (1 << 4),

        /// <summary>
        /// Specifies a PVMI chunk is present, which contains the original filenames of the textures converted to PVR.
        /// </summary>
        PvmiChunk = (1 << 5),

        /// <summary>
        /// Specifies a CONV chunk is present, which contains the name of the converter used to convert textures to PVR.
        /// </summary>
        ConvChunk = (1 << 6),

        /// <summary>
        /// Specifies IMGC chunks are present, which contains the original data of the textures converted to PVR.
        /// </summary>
        ImgcChunks = (1 << 7),

        /// <summary>
        /// Specifies PVRT chunks are present.
        /// </summary>
        PvrtChunks = (1 << 8),
    }
}
