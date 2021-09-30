using System;

namespace Operations.Shared.Exceptions
{
    public class PaymentServiceException : Exception
	{
		public PaymentServiceException(string message) : base(message)
		{

		}

		public PaymentServiceException(string message, Exception exception) : base(message, exception)
		{

		}
	}
}
