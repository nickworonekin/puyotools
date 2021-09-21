using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.App.Tools
{
    class ArchiveExtractorOptions
    {
        public bool DecompressSourceArchive { get; set; }

        public bool ExtractToSourceDirectory { get; set; }

        public bool ExtractToSameNameDirectory { get; set; }

        public bool ExtractFileStructure { get; set; }

        public bool DeleteSourceArchive { get; set; }

        public bool DecompressExtractedFiles { get; set; }

        public bool FileNumberAsFilename { get; set; }

        public bool PrependFileNumber { get; set; }

        public bool ExtractExtractedArchives { get; set; }

        public bool ConvertExtractedTextures { get; set; }
    }
}
