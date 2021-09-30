using System.Threading;
using System.Threading.Tasks;
using Operations.Shared.Models.OrderService;

namespace Operations.Infrastructure.Services
{
    public interface IOrderService
	{
        Task<OrderResult> GetByIdAsync(long orderId, CancellationToken cancellationToken);

        Task CancelAsync(long orderId, CancellationToken cancellationToken);
    }
}