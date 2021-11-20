/*
 * Source: https://source.dot.net/#System.Private.CoreLib/CallerArgumentExpressionAttribute.cs
 * 
 * Provides an implementation of CallerArgumentExpressionAttribute for .NET versions below 5.0
 * when using C# 10+.
 */

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}
#endif