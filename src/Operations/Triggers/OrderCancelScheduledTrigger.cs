using MediatR;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Operations.Infrastructure.Events;
using Operations.Shared.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Operations.Triggers
{
    public class OrderCancelScheduledTrigger
    {
        private readonly IMediator _mediator;

        public OrderCancelScheduledTrigger(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(OrderCancelScheduledTrigger))]
        public async Task RunAsync(
            [ServiceBusTrigger("%topicName%", MessageReceiverQueueName.OrderCancelScheduled, Connection = "SchedulerServiceBusConnection")]
            Message message, ILogger logger)
        {
            var cancelOrder = ConstructScheduledCancelOrderCommand(message);

            try
            {
                logger.LogDebug(
                    $"Received message from Service Bus with id '{message.MessageId}' and body '{Encoding.UTF8.GetString(message.Body)}'");

                await _mediator.Publish(cancelOrder);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    $"Error occurred while canceling Order#{cancelOrder.OrderId}.");

                throw;
            }
        }

        private static OrderCancelScheduledCommand ConstructScheduledCancelOrderCommand(Message message)
        {
            var command = JsonConvert.DeserializeObject<OrderCancelScheduledCommand>(Encoding.UTF8.GetString(message.Body));

            if (message.UserProperties.TryGetValue("CorrelationId", out object correlationId))
            {
                command.CorrelationId = correlationId.ToString();
            }

            return command;
        }
    }
}
