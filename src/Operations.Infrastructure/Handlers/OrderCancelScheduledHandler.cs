using MediatR;
using Microsoft.Extensions.Logging;
using Operations.Infrastructure.Events;
using Operations.Infrastructure.Services;
using Operations.Shared.Extensions;
using Operations.Shared.Models.Enums;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Operations.Infrastructure.Handlers
{
    public class OrderCancelScheduledHandler : INotificationHandler<OrderCancelScheduledCommand>
    {
        private readonly ILogger<OrderCancelScheduledHandler> _logger;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public OrderCancelScheduledHandler(ILogger<OrderCancelScheduledHandler> logger, 
            IOrderService orderService, IPaymentService paymentService)
        {
            _logger = logger;
            _orderService = orderService;
            _paymentService = paymentService;
        }

        public async Task Handle(OrderCancelScheduledCommand notification, CancellationToken cancellationToken)
        {
            notification.OrderId.ThrowIfDefault("OrderId");

            _logger.LogDebug($"Retreiving Order#{notification.OrderId}");

            var order = await _orderService.GetByIdAsync(notification.OrderId, cancellationToken);

            if (order?.OrderId > 0 && order.Status == OrderStatusEnum.Pending)
            {
                var payments = await _paymentService.GetByOrderAsync(notification.OrderId, cancellationToken);

                if (payments.All(x => x.Method == PaymentMethod.CreditCard) 
                    && payments.All(x => x.Status == PaymentStatus.Pending))
                {
                    _logger.LogInformation($"Cancelling Order#{notification.OrderId}");

                    await _orderService.CancelAsync(notification.OrderId, cancellationToken);
                }
            }
            else
            {
                _logger.LogDebug($"Unable to cancel Order#{notification.OrderId}");
            }
        }
    }
}
