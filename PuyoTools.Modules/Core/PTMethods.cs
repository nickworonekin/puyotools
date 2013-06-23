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

        /// <summary>
        /// Checks to see if the array contains the values stored in compareTo at the specified index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Array to check</param>
        /// <param name="sourceIndex">Position in the array to check.</param>
        /// <param name="compareTo">The array to compare to.</param>
        /// <returns>True if the values in compareTo are in the array at the specified index.</returns>
        public static bool Contains<T>(T[] source, int sourceIndex, T[] compareTo)
        {
            if (source == null || compareTo == null)
                return false;

            if (sourceIndex < 0 || sourceIndex + compareTo.Length > source.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < compareTo.Length; i++)
            {
                if (!comparer.Equals(source[sourceIndex + i], compareTo[i])) return false;
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

        public static short ToInt16BE(byte[] value, int startIndex)
        {
            return (short)(value[startIndex] << 8 | value[startIndex + 1]);
        }

        public static int ToInt32BE(byte[] value, int startIndex)
        {
            return (value[startIndex] << 24 | value[startIndex + 1] << 16 | value[startIndex + 2] << 8 | value[startIndex + 3]);
        }
    }
}