using System;

namespace Extensions
{
    // Number Extensions
    public static class Endian
    {
        // Swap Endian
        public static ushort SwapEndian(this ushort x)
        {
            return (ushort)((x >> 8) |
                (x << 8));
        }
        public static ushort SwapEndian(this short x)
        {
            return ((ushort)x).SwapEndian();
        }
        public static uint SwapEndian(this uint x)
        {
            return (x >> 24) |
                ((x << 8) & 0x00FF0000) |
                ((x >> 8) & 0x0000FF00) |
                (x << 24);
        }
        public static uint SwapEndian(this int x)
        {
            return ((uint)x).SwapEndian();
        }

        // Swap Endian
        public static ushort Swap(short x)
        {
            return x.SwapEndian();
        }
        public static ushort Swap(ushort x)
        {
            return x.SwapEndian();
        }
        public static uint Swap(int x)
        {
            return x.SwapEndian();
        }
        public static uint Swap(uint x)
        {
            return x.SwapEndian();
        }
    }

    public static class Number
    {
        // Round number up to a multiple
        public static uint RoundUp(this uint number, int multiple)
        {
            // Return the same number if it is already a multiple
            if (number % multiple == 0)
                return number;

            return (uint)(number + (multiple - (number % multiple)));
        }
        public static int RoundUp(this int number, int multiple)
        {
            // Return the same number if it is already a multiple
            if (number % multiple == 0)
                return number;

            return number + (multiple - (number % multiple));
        }

        // Get amount of digits in a number
        public static int Digits(this int number)
        {
            return number.ToString().Length;
        }
        public static int Digits(this long number)
        {
            return number.ToString().Length;
        }
    }
}