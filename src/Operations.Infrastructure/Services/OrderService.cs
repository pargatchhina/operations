using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Operations.Shared.Models.OrderService;
using Operations.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System.Threading;
using Operations.Shared.Models.Enums;

namespace Operations.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
		private readonly HttpClient _httpClient;
        private readonly ILogger<OrderService> _logger;

        public OrderService(HttpClient httpClient, ILogger<OrderService> logger)
		{
			_httpClient = httpClient;
            _logger = logger;
		}

        public async Task CancelAsync(long orderId, CancellationToken cancellationToken)
        {
            try
            {
                var content = JsonConvert.SerializeObject(new
                {
                    Status = OrderStatusEnum.Cancelled
                });

                var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"v1/orders/{orderId}/status", stringContent, cancellationToken);

                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new FailedToCancelOrderException($"Failed to cancel order {orderId} , HttpStatusCode {response.StatusCode}");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error occured while cancelling Order#{orderId}");
                throw new FailedToCancelOrderException($"Error occured while cancelling Order#{orderId}", exception);
            }
        }

        public async Task<OrderResult> GetByIdAsync(long orderId, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"v1/orders/{orderId}");
            var response = await _httpClient.SendAsync(request, cancellationToken);


            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new EntityNotFoundException($"Order not found, order {orderId}.");
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new FailedToGetOrderException(
                    $"Failed to retrieve order '{orderId}', StatusCode '{response.StatusCode}'.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var orderResult = JsonConvert.DeserializeObject<OrderResult>(content);

            return orderResult;
        }
    }
}