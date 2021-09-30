using MediatR;

namespace Operations.Infrastructure.Events
{
    public class OrderCancelScheduledCommand : BaseMessage, INotification
    {
        public long OrderId { get; set; }
    }
}
