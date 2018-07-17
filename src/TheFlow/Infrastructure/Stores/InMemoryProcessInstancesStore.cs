using System;
using System.Collections.Concurrent;
using System.Linq;
using TheFlow.CoreConcepts;

namespace TheFlow.Infrastructure.Stores
{
    public class InMemoryProcessInstancesStore
        : IProcessInstancesStore
    {
        private readonly ConcurrentDictionary<string, ProcessInstance> _instances =
            new ConcurrentDictionary<string, ProcessInstance>();
        
        public ProcessInstance GetById(Guid id)
        {
            return _instances.ContainsKey(id.ToString()) ? _instances[id.ToString()] : null;
        }

        public void Store(ProcessInstance instance)
        {
            _instances.AddOrUpdate(
                instance.Id,
                instance,
                (instanceId, oldValue) => instance
            );
        }

        public int GetRunningInstancesCount() => 
            _instances.Values.Count(instance => instance.IsRunning);

        public ProcessInstance GetProcessInstance(Guid id) => GetById(id);
    }
}