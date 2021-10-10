using PuyoTools.App.Formats.Textures;
using PuyoTools.App.Formats.Textures.WriterSettings;
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
    /// <inheritdoc/>
    internal partial class PvrFormat : ITextureFormat
    {
        public ModuleSettingsControl GetModuleSettingsControl() => new PvrWriterSettings();
    }
}
