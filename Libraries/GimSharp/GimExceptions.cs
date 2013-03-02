using System;
using System.Collections.Generic;
using System.Text;

namespace GimSharp
{
    public class GimNoSuitableCodecException : _ErrorException
    {
        public GimNoSuitableCodecException(string errorMessage) : base(errorMessage) { }

        public GimNoSuitableCodecException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
    }
    public class GimNoSuitablePaletteException : _ErrorException
    {
        public GimNoSuitablePaletteException(string errorMessage) : base(errorMessage) { }

        public GimNoSuitablePaletteException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
    }
    public class GimCodecProcessingException : _ErrorException
    {
        public GimCodecProcessingException(string errorMessage) : base(errorMessage) { }

        public GimCodecProcessingException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
    }
    public class GimCodecHeaderException : _ErrorException
    {
        public GimCodecHeaderException(string errorMessage) : base(errorMessage) { }

        public GimCodecHeaderException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
    }
    public class GimCodecLoadingException : _ErrorException
    {
        public GimCodecLoadingException(string errorMessage) : base(errorMessage) { }

        public GimCodecLoadingException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
    }
    public class NotGimException : _ErrorException
    {
        public NotGimException(string errorMessage) : base(errorMessage) { }

        public NotGimException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { }
    }

    [Serializable]
    public class _ErrorException : Exception
    {
        public string ErrorMessage
        {
            get
            {
                return base.Message.ToString();
            }
        }

        public _ErrorException(string errorMessage)
            : base(errorMessage) { }

        public _ErrorException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx) { }
    }
}