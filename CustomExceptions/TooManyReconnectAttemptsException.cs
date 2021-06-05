using System;
using System.Collections.Generic;
using System.Text;

namespace APIDataHelper
{
    public class TooManyReconnectAttemptsException : Exception
    {
        public TooManyReconnectAttemptsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
