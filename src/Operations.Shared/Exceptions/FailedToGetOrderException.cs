using System;

namespace Operations.Shared.Exceptions
{
    public class FailedToGetOrderException : Exception
    {
        public FailedToGetOrderException(string message) : base(message)
        {
        }

        public FailedToGetOrderException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
