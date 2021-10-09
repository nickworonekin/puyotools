using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Archive
{
    public class FileRejectedException : Exception
    {
        public FileRejectedException(string message) : base(message)
        {
        }
    }
}