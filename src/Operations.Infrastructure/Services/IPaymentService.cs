using Operations.Shared.Models.PaymentService;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Operations.Infrastructure.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResult>> GetByOrderAsync(long orderId, CancellationToken cancellationToken);
    }
}
