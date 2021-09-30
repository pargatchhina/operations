using System;

namespace Operations.Shared.Exceptions
{
    public class FailedToCancelOrderException : Exception
    {
        public FailedToCancelOrderException(string message) : base(message)
        {
        }

        public FailedToCancelOrderException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}