using PuyoTools.GUI;
using PuyoTools.Core.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.App.Formats.Textures
{
    internal partial interface ITextureFormat
    {
        /// <summary>
        /// Gets the <see cref="ModuleSettingsControl"/> for this format, or null if it does not have one.
        /// </summary>
        /// <returns></returns>
        ModuleSettingsControl GetModuleSettingsControl();
    }
}
