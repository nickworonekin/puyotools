using System;
using System.ComponentModel.Composition;

namespace PuyoTools
{
    [InheritedExport(typeof(IPuyoToolsPlugin))]
    public interface IPuyoToolsPlugin
    {
    }
}