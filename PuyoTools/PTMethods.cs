using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PuyoTools2
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
        /// Checks to see if the stream contains the specified sequence of characters.
        /// </summary>
        /// <param name="inStream">Stream to check</param>
        /// <param name="offset">Offset to check (relative to the current stream position)</param>
        /// <param name="length">Number of bytes to check.</param>
        /// <param name="a">Array to compare to</param>
        /// <returns></returns>
        public static bool StreamContains(Stream inStream, long offset, char[] a)
        {
            // First, let's do the logical thing and make sure a.Length > 0 and a != null
            if (a.Length > 0 && a == null)
                return false;

            // Go to the offset we want to check.
            // In this case, offset is relative to the position of the stream
            long oldPosition = inStream.Position;
            inStream.Position += offset;

            // Read in the buffer now
            byte[] buffer = new byte[a.Length];
            inStream.Read(buffer, 0, a.Length);

            // Reset the position of the stream back to oldPosition
            inStream.Position = oldPosition;

            // Now let's check to see if the stream contains a
            return ArraysEqual<byte>(buffer, Encoding.UTF8.GetBytes(a));
        }
    }
}