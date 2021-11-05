using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PuyoTools.Core
{
    internal static class EnumHelper
    {
        /// <summary>
        /// Throws <see cref="InvalidEnumArgumentException"/> if <paramref name="value"/> is not defined in <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static void ThrowIfArgumentNotDefined<T>(T value, string paramName) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                throw new InvalidEnumArgumentException(paramName, (int)(object)value, typeof(T));
            }
        }
    }
}
