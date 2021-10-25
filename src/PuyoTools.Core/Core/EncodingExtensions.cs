using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core
{
    internal static class EncodingExtensions
    {
        private static readonly Lazy<Encoding> shiftJisEncoding = new Lazy<Encoding>(() => Encoding.GetEncoding("Shift_JIS"));
        private static readonly Lazy<Encoding> utf8NoBomEncoding = new Lazy<Encoding>(() => new UTF8Encoding(false));

#if (NET || NETCOREAPP || NETSTANDARD) 
        static EncodingExtensions()
        {
            // Register the CodePagesEncodingProvider for Shift JIS encoding support
            // This is only needed when targeting .NET 5.0+/.NET Core/.NET Standard.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif

        /// <summary>
        /// Gets an encoding for the Shift JIS format.
        /// </summary>
        public static Encoding ShiftJIS => shiftJisEncoding.Value;

        /// <summary>
        /// Gets an encoding for the UTF-8 format with no Unicode byte order mark.
        /// </summary>
        public static Encoding UTF8NoBOM => utf8NoBomEncoding.Value;
    }
}
