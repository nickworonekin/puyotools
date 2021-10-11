using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Textures.Gim
{
    public class GimMetadata
    {
        /// <summary>
        /// Returns the original filename of the texture specified in the metadata.
        /// </summary>
        public string OriginalFilename { get; internal set; }

        /// <summary>
        /// Returns the user that created this texture as specified in the metadata.
        /// </summary>
        public string User { get; internal set; }

        /// <summary>
        /// Returns the timestamp in the metadata. The timestamp is in the format of "ddd MMM d HH:mm:ss yyyy".
        /// </summary>
        public string Timestamp { get; internal set; }

        /// <summary>
        /// Returns the program used to create this texture as specified in the metadata.
        /// </summary>
        public string Program { get; internal set; }
    }
}
