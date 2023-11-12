using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core
{
    internal static class EncodingExtensions
    {
        private static readonly Lazy<Encoding> shiftJisEncoding = new(() => Encoding.GetEncoding("Shift_JIS"));
        private static readonly Lazy<Encoding> utf8NoBomEncoding = new(() => new UTF8Encoding(false));

        static EncodingExtensions()
        {
            // Register the CodePagesEncodingProvider for Shift JIS encoding support.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

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
