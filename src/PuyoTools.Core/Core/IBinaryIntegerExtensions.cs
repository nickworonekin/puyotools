using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PuyoTools.Core
{
    public static class IBinaryIntegerExtensions
    {
        extension<T>(IBinaryInteger<T>)
            where T : IBinaryInteger<T>
        {
            public static T RoundUp(T value, T multipleOf)
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(multipleOf);

                return (value + multipleOf - T.One) / multipleOf * multipleOf;
            }
        }
    }
}
