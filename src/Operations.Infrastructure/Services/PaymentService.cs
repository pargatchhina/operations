using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Operations.Shared.Exceptions;
using Operations.Shared.Models.PaymentService;
using Operations.Shared.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Operations.Infrastructure.Services
{
    public class PaymentService : IPaymentService
	{
		private readonly ILogger<PaymentService> _logger;
		private readonly HttpClient _httpClient;
        private readonly PaymentServiceOptions _options;

        public PaymentService(ILogger<PaymentService> logger, HttpClient httpClient, 
			IOptions<PaymentServiceOptions> options)
		{
			_logger = logger;
			_httpClient = httpClient;
			_options = options.Value;
        }

        public async Task<IEnumerable<PaymentResult>> GetByOrderAsync(long orderId, CancellationToken cancellationToken)
		{
			IEnumerable<PaymentResult> paymentResult;

			try
			{
				var response = await _httpClient.GetAsync($"v1/orders/{orderId}/payments?code={_options.Code}", cancellationToken);

				if (response.StatusCode == HttpStatusCode.NotFound)
				{
					throw new EntityNotFoundException($"Payments not found for Order#{orderId}");
				}

				var content = await response.Content.ReadAsStringAsync();

				paymentResult = JsonConvert.DeserializeObject<IEnumerable<PaymentResult>>(content);
			}
			catch (Exception exception)
			{
				_logger.LogError(
					$"Error occured while getting payments for Order#{orderId}, Message '{exception.Message}'",
					exception);

				throw new PaymentServiceException(exception.Message, exception);
			}

			return paymentResult;
		}
    }
}