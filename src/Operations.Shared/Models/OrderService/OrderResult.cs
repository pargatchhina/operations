using Operations.Shared.Models.Enums;

namespace Operations.Shared.Models.OrderService
{
    public class OrderResult
    {
        public long OrderId { get; set; }

        public OrderStatusEnum Status { get; set; }
    }
}
