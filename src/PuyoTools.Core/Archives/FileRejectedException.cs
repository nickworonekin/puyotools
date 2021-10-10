using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Archives
{
    public class FileRejectedException : Exception
    {
        public FileRejectedException(string message) : base(message)
        {
        }
    }
}