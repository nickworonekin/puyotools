using System.Runtime.CompilerServices;

// Implementation from .NET source code:
// https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/Buffers/Binary/Reader.cs

namespace PuyoTools.Modules
{
    /// <summary>
    /// Reads bytes as primitives with specific endianness
    /// </summary>
    public static partial class BinaryPrimitives
    {
        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReverseEndianness(short value) => (short)ReverseEndianness((ushort)value);

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReverseEndianness(int value) => (int)ReverseEndianness((uint)value);

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReverseEndianness(long value) => (long)ReverseEndianness((ulong)value);

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReverseEndianness(ushort value)
        {
            return (ushort)((value >> 8) + (value << 8));
        }

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReverseEndianness(uint value)
        {
            return ((value & 0x00FF00FFu) >> 8) | ((value & 0x00FF00FFu) << 24)
                + ((value & 0xFF00FF00u) << 8) | ((value & 0xFF00FF00u) >> 24);
        }

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReverseEndianness(ulong value)
        {
            return ((ulong)ReverseEndianness((uint)value) << 32)
                + ReverseEndianness((uint)(value >> 32));
        }
    }
}
