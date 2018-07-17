using System;
using TheFlow.CoreConcepts;

namespace TheFlow.Infrastructure.Stores
{
    public interface IProcessInstancesStore 
        : IProcessInstanceProvider
    {
        ProcessInstance GetById(Guid id);
        void Store(ProcessInstance instance);

        int GetRunningInstancesCount();
    }
}