using System;
using System.Collections.Generic;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;

namespace TheFlow
{
    public interface IProcessManager
    {
        IProcessInstancesStore InstancesStore { get; }
        IProcessModelsStore ModelsStore { get; }

        string GetExecutionPoint(Guid instanceId, Guid tokenId);
        HandleResult HandleEvent(Guid processInstanceId, Guid tokenId, object eventData);
        HandleResult HandleActivityCompletion(Guid processInstanceId, Guid tokenId, object completationData);
        
        IEnumerable<HandleResult> HandleEvent(object e);

        void Attach(ProcessInstance instance);
    }
}