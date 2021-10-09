using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PuyoTools.Core
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryReader"/> class based on the specified stream and using UTF-8 encoding, and leaves the stream open.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryReader AsBinaryReader(this Stream input) => AsBinaryReader(input, Encoding.UTF8);

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryReader"/> class based on the specified stream and character encoding, and leaves the stream open.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryReader AsBinaryReader(this Stream input, Encoding encoding) => new BinaryReader(input, encoding, true);

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWriter"/> class based on the specified stream and using UTF-8 encoding, and leaves the stream open.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryWriter AsBinaryWriter(this Stream output) => AsBinaryWriter(output, Encoding.UTF8);

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWriter"/> class based on the specified stream and character encoding, and leaves the stream open.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryWriter AsBinaryWriter(this Stream output, Encoding encoding) => new BinaryWriter(output, encoding, true);
    }
}
