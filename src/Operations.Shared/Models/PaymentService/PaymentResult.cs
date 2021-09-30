using Operations.Shared.Models.Enums;

namespace Operations.Shared.Models.PaymentService
{
    public class PaymentResult
	{
		public long PaymentId { get; set; }

		public PaymentMethod Method { get; set; }

		public PaymentStatus Status { get; set; }
	}
}
