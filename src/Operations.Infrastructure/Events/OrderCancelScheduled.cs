namespace Operations.Infrastructure.Events
{
    public class OrderCancelScheduled : BaseMessage
    {
        public OrderCancelScheduled(long orderId, string correlationId = null) : base(correlationId)
        {
            OrderId = orderId;
        }

        public long OrderId { get; }
    }
}