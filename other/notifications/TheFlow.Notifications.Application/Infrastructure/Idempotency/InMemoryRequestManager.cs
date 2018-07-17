using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheFlow.Notifications.Application.Infrastructure.Idempotency
{
    public class InMemoryRequestManager : IRequestManager
    {
        private readonly CircularQueue<Guid> _queue = new CircularQueue<Guid>();

        public Task<bool> IsRegistered(Guid id, CancellationToken cancellationToken = default(CancellationToken)) =>
            Task.FromResult(_queue.Contains(id));

        public Task Register(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            _queue.Enqueue(id);
            return Task.CompletedTask;
        }
    }
}
