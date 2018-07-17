using System;
using System.Collections.Generic;
using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;

namespace TheFlow
{
    public class ProcessManager 
        : IProcessManager, IServiceProvider
    {
        public IProcessModelsStore ModelsStore { get; }
        public IProcessInstancesStore InstancesStore { get; }

        public ProcessManager(
            IProcessModelsStore modelsStore,
            IProcessInstancesStore instancesStore
            )
        {
            ModelsStore = modelsStore;
            InstancesStore = instancesStore;
        }

        public IEnumerable<HandleResult> HandleEvent(object e)
        {
            var models = ModelsStore.GetAll()
                .Where(model => model.CanStartWith(e));

            var result = new List<HandleResult>();

            foreach (var model in models)
            {
                var instance = ProcessInstance.Create(Guid.Parse(model.Id));
                var tokens = instance.HandleEvent(model, e);
                
                result.Add(new HandleResult(
                    Guid.Parse(model.Id),
                    Guid.Parse(instance.Id),
                    tokens.Select(token => token.Id).ToList()
                    ));
                
                // TODO: It can be late?
                InstancesStore.Store(instance);
            }
            return result;
        }

        public HandleResult HandleEvent(
            Guid processInstanceId,
            Guid tokenId,
            object eventData
        )
        {
            var instance = InstancesStore.GetById(processInstanceId);
            if (instance == null)
            {
                return null;
            }

            var model = ModelsStore.GetById(Guid.Parse(instance.ProcessModelId));
            if (model == null)
            {
                throw new InvalidOperationException("Instance process model not found.");
            }

            var tokens = instance.HandleEvent(tokenId, model, eventData);
            
            InstancesStore.Store(instance);
            
            return new HandleResult(
                Guid.Parse(model.Id),
                Guid.Parse(instance.Id),
                tokens.Select(token => token.Id).ToList());
        }

        public string GetExecutionPoint(
            Guid instanceId, 
            Guid tokenId
            )
        {
            return InstancesStore.GetById(instanceId)
                ?.Token.FindById(tokenId)?.ExecutionPoint;
        }

        public HandleResult HandleActivityCompletation(
            Guid processInstanceId, 
            Guid tokenId, 
            object completationData
            )
        {
            var instance = InstancesStore.GetById(processInstanceId);
            if (instance == null)
            {
                return null;
            }

            var model = ModelsStore.GetById(Guid.Parse(instance.ProcessModelId));
            if (model == null)
            {
                throw new InvalidOperationException("Instance process model not found.");
            }

            var tokens = instance.HandleActivityCompletation(tokenId, model, completationData);

            InstancesStore.Store(instance);

            return new HandleResult(
                Guid.Parse(model.Id),
                Guid.Parse(instance.Id),
                tokens.Select(token => token.Id).ToList()
                );
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IProcessModelProvider))
                return ModelsStore;

            if (serviceType == typeof(IProcessInstanceProvider))
                return InstancesStore;

            return null;
        }
    }
}