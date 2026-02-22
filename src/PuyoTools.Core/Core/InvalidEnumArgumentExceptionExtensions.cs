using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PuyoTools.Core
{
    public static class InvalidEnumArgumentExceptionExtensions
    {
        extension(InvalidEnumArgumentException)
        {
            /// <summary>Throws an <see cref="InvalidEnumArgumentException"/> if <paramref name="value"/> is not defined in <typeparamref name="T"/>.</summary>
            /// <param name="value">The argument to validate as being defined in <typeparamref name="T"/>.</param>
            /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
            public static void ThrowIfNotDefined<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
                where T : struct, Enum
            {
                if (!Enum.IsDefined(value))
                {
                    throw new InvalidEnumArgumentException(paramName, Convert.ToInt32(value), typeof(T));
                }
            }
        }
    }
}
