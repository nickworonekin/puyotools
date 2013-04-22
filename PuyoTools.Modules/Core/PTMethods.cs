using System;
using System.Collections.Generic;

namespace PuyoTools.Modules
{
    public static class PTMethods
    {
        /// <summary>
        /// Compares two arrays to see if they are equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns>True if the arrays are equal.</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        public static int RoundUp(int value, int roundUpTo)
        {
            // Return the same number if it is already a multiple
            if (value % roundUpTo == 0)
                return value;

            return value + (roundUpTo - (value % roundUpTo));
        }

        public static ushort ToUInt16BE(byte[] value, int startIndex)
        {
            return (ushort)(value[startIndex] << 8 | value[startIndex + 1]);
        }

        public static uint ToUInt32BE(byte[] value, int startIndex)
        {
            return (uint)(value[startIndex] << 24 | value[startIndex + 1] << 16 | value[startIndex + 2] << 8 | value[startIndex + 3]);
        }
    }
}