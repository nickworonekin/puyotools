using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PuyoTools.GUI
{
    public partial class AboutPuyoTools : Form
    {
        public AboutPuyoTools()
        {
            InitializeComponent();

            this.Icon = IconResources.ProgramIcon;

            versionLabel.Text = "Version " + Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/nickworonekin/puyotools");
        }
    }
}
