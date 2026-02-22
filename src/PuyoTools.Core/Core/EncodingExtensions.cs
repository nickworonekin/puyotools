using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Core
{
    internal static class EncodingExtensions
    {
        //private static readonly Encoding s_shiftJisEncoding = Encoding.GetEncoding("Shift_JIS");
        private static readonly Encoding s_shiftJisEncoding = CodePagesEncodingProvider.Instance.GetEncoding("Shift_JIS")!;
        private static readonly Encoding s_utf8NoBomEncoding = new UTF8Encoding(false);

        static EncodingExtensions()
        {
            // Register the CodePagesEncodingProvider for Shift JIS encoding support.
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        extension(Encoding)
        {
            /// <summary>
            /// Gets an encoding for the Shift JIS format.
            /// </summary>
            public static Encoding ShiftJIS => s_shiftJisEncoding;

            /// <summary>
            /// Gets an encoding for the UTF-8 format with no Unicode byte order mark.
            /// </summary>
            public static Encoding UTF8NoBOM => s_utf8NoBomEncoding;
        }
    }
}
