using PuyoTools.GUI;
using PuyoTools.Core;
using PuyoTools.Core.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Archives
{
    internal partial interface IArchiveFormat
    {
        /// <summary>
        /// Gets the <see cref="ModuleSettingsControl"/> for this format, or null if it does not have one.
        /// </summary>
        /// <returns></returns>
        ModuleSettingsControl GetModuleSettingsControl();
    }
}
