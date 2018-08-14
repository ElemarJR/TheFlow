using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheFlow.Notifications.Application.Infrastructure.Idempotency
{
    public interface IRequestManager
    {
        Task<bool> IsRegistered(
            Guid id);

        Task<bool> IsRegistered(
            Guid id,
            CancellationToken cancellationToken);

        Task Register(Guid id);

        Task Register(Guid id,
            CancellationToken cancellationToken
            );
    }
}
