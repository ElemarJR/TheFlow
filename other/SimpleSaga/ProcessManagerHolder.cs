using System;
using TheFlow;
using TheFlow.Infrastructure.Stores;

namespace SimpleSaga
{
    public static class ProcessManagerHolder
    {
        private static readonly Lazy<ProcessManager> LazyProcessManager = 
            new Lazy<ProcessManager>(() =>
            {
                var models = new InMemoryProcessModelsStore();
                var instances = new InMemoryProcessInstancesStore();

                return new ProcessManager(models, instances);
            });

        public static ProcessManager Instance =>
            LazyProcessManager.Value;
    }
}