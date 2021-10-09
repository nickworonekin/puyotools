using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core.Core
{
    public static class MathExtensions
    {
        public static int RoundUp(int value, int multiple)
        {
            // Return the same number if it is already a multiple
            if (value % multiple == 0)
                return value;

            return value + (multiple - (value % multiple));
        }
    }
}
