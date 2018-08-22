using System;
using System.Threading;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.CompareExchange;
using TheFlow.Infrastructure.Parallel;

namespace TheFlow.Infrastructure.Stores.InstancesStore.RavenDB
{
    public class DistributedLockObject {}
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
                var lockObject = new DistributedLockObject();

                var result = _documentStore.Operations.Send(
                    new PutCompareExchangeValueOperation<DistributedLockObject>(
                        lockKey, lockObject, 0)
                    );

                if (result.Successful)
                {
                    // resourceName wasn't present - we managed to reserve
                    return result.Index;
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
