using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuyoTools.Core.Textures.Quantizers.Wu
{
    public class QuantizationException : ApplicationException
    {
        public QuantizationException(string message) : base(message)
        {

        }
    }
}
