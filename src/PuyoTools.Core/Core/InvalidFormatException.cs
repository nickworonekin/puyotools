using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PuyoTools.Core
{
    /// <summary>
    /// The exception that is thrown when the data is not in the expected format.
    /// </summary>
    public class InvalidFormatException : Exception
    {
        public InvalidFormatException()
        {
        }

        public InvalidFormatException(string message) : base(message)
        {
        }

        public InvalidFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
