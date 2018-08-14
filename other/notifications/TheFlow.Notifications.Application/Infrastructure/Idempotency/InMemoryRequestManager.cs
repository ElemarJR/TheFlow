using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheFlow.Notifications.Application.Infrastructure.Idempotency
{
    public class InMemoryRequestManager : IRequestManager
    {
        private readonly CircularQueue<Guid> _queue = new CircularQueue<Guid>();

        public Task<bool> IsRegistered(Guid id)
        {
            return IsRegistered(id, new CancellationToken());
        }

        public Task<bool> IsRegistered(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(_queue.Contains(id));

        public Task Register(Guid id)
        {
            return Register(id, new CancellationToken());
        }

        public Task Register(Guid id, CancellationToken cancellationToken)
        {
            _queue.Enqueue(id);
            return Task.CompletedTask;
        }
    }
}
