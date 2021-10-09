using System;
using System.Collections.Generic;

namespace PuyoTools.Core
{
    public static class PTMethods
    {
        public static int RoundUp(int value, int roundUpTo)
        {
            // Return the same number if it is already a multiple
            if (value % roundUpTo == 0)
                return value;

            return value + (roundUpTo - (value % roundUpTo));
        }
    }
}