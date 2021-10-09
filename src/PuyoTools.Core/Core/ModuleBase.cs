using System;
using System.Collections.Generic;
using System.IO;

namespace PuyoTools.Core
{
    public interface IModule { }

    public abstract class ModuleBase : IModule
    {
        /// <summary>
        /// The source path of the file.
        /// </summary>
        //public string SourcePath = String.Empty;

        /// <summary>
        /// The destination path of the file.
        /// </summary>
        //public string DestinationPath = String.Empty;
    }
}