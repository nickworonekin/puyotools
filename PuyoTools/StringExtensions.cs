using System;
using System.Text.RegularExpressions;

namespace Extensions
{
    public static class StringExtensions
    {
        // Check if the string contains only uppercase characters
        public static bool IsAllUpperCase(this string str)
        {
            return !(new Regex("[a-z]").IsMatch(str));
        }
    }
}