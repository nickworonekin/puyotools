using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.Modules
{
    internal static class EncodingExtensions
    {
        private static Encoding shiftJisEncoding;
        private static Encoding utf8NoBomEncoding;

#if NETSTANDARD2_0
        static EncodingExtensions()
        {
            // Register the CodePagesEncodingProvider for Shift JIS encoding support
            // This is only needed when targeting .NET Standard.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif

        /// <summary>
        /// Gets an encoding for the Shift JIS format.
        /// </summary>
        public static Encoding ShiftJIS
        {
            get
            {
                if (shiftJisEncoding == null)
                {
                    shiftJisEncoding = Encoding.GetEncoding("Shift_JIS");
                }

                return shiftJisEncoding;
            }
        }

        /// <summary>
        /// Gets an encoding for the UTF-8 format with no Unicode byte order mark.
        /// </summary>
        public static Encoding UTF8NoBOM
        {
            get
            {
                if (utf8NoBomEncoding == null)
                {
                    utf8NoBomEncoding = new UTF8Encoding(false);
                }

                return utf8NoBomEncoding;
            }
        }
    }
}
