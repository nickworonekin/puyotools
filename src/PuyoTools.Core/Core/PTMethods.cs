using System;
using System.Collections.Generic;
using System.Text;

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

        public static string StringFromBytes(byte[] source, int index)
        {
            int length = 0;
            while (source[index + length] != 0)
            {
                length++;
            }

            if (length == 0)
            {
                return String.Empty;
            }

            return Encoding.ASCII.GetString(source, index, length);
        }
    }
}