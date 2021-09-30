using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Operations.Infrastructure.Events;
using Operations.Infrastructure.Handlers;
using Operations.Infrastructure.Services;
using Operations.Shared.Models.Enums;
using Operations.Shared.Models.OrderService;
using Operations.Shared.Models.PaymentService;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Operations.UnitTests.Infrastructure.Handlers
{
    public class OrderCancelScheduledHandlerTests
    {
        OrderCancelScheduledHandler _sut;
        Mock<ILogger<OrderCancelScheduledHandler>> _logger;
        Mock<IOrderService> _orderService;
        Mock<IPaymentService> _paymentService;

        public OrderCancelScheduledHandlerTests()
        {
            _logger = new Mock<ILogger<OrderCancelScheduledHandler>>();
            _orderService = new Mock<IOrderService>();
            _paymentService = new Mock<IPaymentService>();

            _sut = new OrderCancelScheduledHandler(_logger.Object, _orderService.Object, _paymentService.Object);
        }

        [Fact]
        public async Task Given_orderId_is_zero_should_throw_error()
        {
            var orderCancelScheduledCommand = new OrderCancelScheduledCommand
            {
                OrderId = 0
            };

            Func<Task> act = () => _sut.Handle(orderCancelScheduledCommand, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task Given_order_is_not_pending_should_not_cancel_order()
        {
            var orderCancelScheduledCommand = new OrderCancelScheduledCommand
            {
                OrderId = 123
            };

            var orderResult = new OrderResult() { OrderId = orderCancelScheduledCommand.OrderId };

            _orderService.Setup(x => x.GetByIdAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResult);

            await  _sut.Handle(orderCancelScheduledCommand, CancellationToken.None);

            _paymentService.Verify(x => x.GetByOrderAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()), Times.Never);

            _orderService.Verify(x => x.CancelAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(PaymentMethod.Bacs)]
        public async Task Given_order_is_pending_for_non_credit_card_payment_should_not_cancel_order(PaymentMethod method)
        {
            var orderCancelScheduledCommand = new OrderCancelScheduledCommand
            {
                OrderId = 123
            };

            var orderResult = new OrderResult { OrderId = orderCancelScheduledCommand.OrderId, Status =  OrderStatusEnum.Pending};

            var payments = new List<PaymentResult>
            {
                new PaymentResult { Method = method }
            };

            _orderService.Setup(x => x.GetByIdAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResult);

            _paymentService.Setup(x => x.GetByOrderAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments);

            await _sut.Handle(orderCancelScheduledCommand, CancellationToken.None);

            _paymentService.Verify(x => x.GetByOrderAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()), Times.Once);

            _orderService.Verify(x => x.CancelAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Given_order_is_pending_for_non_pending_payment_should_not_cancel_order()
        {
            var orderCancelScheduledCommand = new OrderCancelScheduledCommand
            {
                OrderId = 12345
            };

            var orderResult = new OrderResult { OrderId = orderCancelScheduledCommand.OrderId, Status = OrderStatusEnum.Pending };

            var payments = new List<PaymentResult>
            {
                new PaymentResult { Method = PaymentMethod.CreditCard, Status = PaymentStatus.Pending },
                new PaymentResult { Method = PaymentMethod.CreditCard, Status = PaymentStatus.Processed }
            };

            _paymentService.Setup(x => x.GetByOrderAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments);

            _orderService.Setup(x => x.GetByIdAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResult);

            await _sut.Handle(orderCancelScheduledCommand, CancellationToken.None);

            _paymentService.Verify(x => x.GetByOrderAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()), Times.Once);

            _orderService.Verify(x => x.CancelAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Given_order_is_pending_for_pending_payment_should_cancel_order()
        {
            var orderCancelScheduledCommand = new OrderCancelScheduledCommand
            {
                OrderId = 12345
            };

            var orderResult = new OrderResult { OrderId = orderCancelScheduledCommand.OrderId, Status = OrderStatusEnum.Pending };

            var payments = new List<PaymentResult>
            {
                new PaymentResult { Method = PaymentMethod.CreditCard, Status = PaymentStatus.Pending }
            };
            _paymentService.Setup(x => x.GetByOrderAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments);

            _orderService.Setup(x => x.GetByIdAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResult);

            await _sut.Handle(orderCancelScheduledCommand, CancellationToken.None);

            _paymentService.Verify(x => x.GetByOrderAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()), Times.Once);

            _orderService.Verify(x => x.CancelAsync(orderCancelScheduledCommand.OrderId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
