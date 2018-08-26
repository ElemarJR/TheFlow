using System;
using System.Threading;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.CompareExchange;
using TheFlow.Infrastructure.Parallel;

namespace TheFlow.Infrastructure.Stores.InstancesStore.RavenDB
{
    public class DistributedLockObject
    {
        public DateTime? ExpiresAt { get; set; }
    }

    public class RavenDbProcessMonitor : IProcessMonitor
    {
        private readonly IDocumentStore _documentStore;

        public RavenDbProcessMonitor(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public long Enter(string lockKey)
        {
            while (true)
            {
                var now = DateTime.UtcNow;

                var lockObject = new DistributedLockObject
                {
                    ExpiresAt = DateTime.UtcNow + TimeSpan.FromMinutes(5)
                };

                var result = _documentStore.Operations.Send(
                    new PutCompareExchangeValueOperation<DistributedLockObject>(
                        lockKey, lockObject, 0)
                    );

                if (result.Successful)
                {
                    // resourceName wasn't present - we managed to reserve
                    return result.Index;
                }

                if (result.Value.ExpiresAt < now)
                {
                    // Time expired - Update the existing key with the new value
                    var takeLockWithTimeoutResult = _documentStore.Operations.Send(
                        new PutCompareExchangeValueOperation<DistributedLockObject>(lockKey, lockObject, result.Index));

                    if (takeLockWithTimeoutResult.Successful)
                    {
                        return takeLockWithTimeoutResult.Index;
                    }
                }
                
                // Wait a little bit and retry
                Thread.Sleep(20);
            }
        }

        public void Exit(string lockKey, long lockIndex)
        {
            _documentStore.Operations.Send(
                new DeleteCompareExchangeValueOperation<DistributedLockObject>(
                    lockKey, 
                    lockIndex)
                );
        }

        public IDisposable Lock(string lockKey)
            => new ProcessLock(this, lockKey);
    }
}
