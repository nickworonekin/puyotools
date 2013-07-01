using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.Modules
{
    public partial class ModuleSettingsControl : UserControl
    {
        public ModuleSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets a module's settings.
        /// </summary>
        /// <param name="module"></param>
        public virtual void SetModuleSettings(IModule module) { }
    }
}
