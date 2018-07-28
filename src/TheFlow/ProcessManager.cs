using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TheFlow.CoreConcepts;
using TheFlow.Infrastructure.Stores;

namespace TheFlow
{
    public class ProcessManager 
        : IProcessManager, IServiceProvider
    {
        private readonly IServiceCollection _serviceCollection;
        public IProcessModelsStore ModelsStore { get; }
        public IProcessInstancesStore InstancesStore { get; }

        public ProcessManager(
            IProcessModelsStore modelsStore,
            IProcessInstancesStore instancesStore,
            IServiceCollection serviceCollection = null
            )
        {
            _serviceCollection = serviceCollection;
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
                var context = new ExecutionContext(this, model, instance, instance.Token, null);

                var tokens = instance.HandleEvent(context, e);
                
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

        // TODO: Verify if instance is valid (not null and with a valid model)
        public void Attach(ProcessInstance instance)
        {
            InstancesStore.Store(instance);
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
            
            var context = new ExecutionContext(this, model, instance, instance.Token.FindById(tokenId), null);

            var tokens = instance.HandleEvent(context, eventData);
            
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

        public HandleResult HandleActivityCompletion(
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
            
            var context = new ExecutionContext(this, model, instance, instance.Token.FindById(tokenId), null); 

            var tokens = instance.HandleActivityCompletion(context, completationData);

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

            return _serviceCollection?.BuildServiceProvider()?.GetService(serviceType);
        }
    }
}