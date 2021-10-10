using PuyoTools.Formats.Archives.WriterSettings;
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
    /// <inheritdoc/>
    internal partial class AfsFormat : IArchiveFormat
    {
        public ModuleSettingsControl GetModuleSettingsControl() => new AfsWriterSettings();
    }
}
