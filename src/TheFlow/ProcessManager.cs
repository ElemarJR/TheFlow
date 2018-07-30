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
        private readonly IServiceProvider _serviceProvider;
        public IProcessModelsStore ModelsStore { get; }
        public IProcessInstancesStore InstancesStore { get; }

        public ProcessManager(
            IProcessModelsStore modelsStore,
            IProcessInstancesStore instancesStore,
            IServiceProvider serviceProvider = null
            )
        {
            _serviceProvider = serviceProvider;
            ModelsStore = modelsStore;
            InstancesStore = instancesStore;

        }

        public IEnumerable<HandleResult> HandleEvent(object e)
        {
            var models = ModelsStore.GetAll()
                .Where(model =>
                {
                    var context = new ExecutionContext(this, model, null, null, null);
                    return model.CanStartWith(context, e);
                });

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

        public void Attach(ProcessInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (ModelsStore.GetById(Guid.Parse(instance.ProcessModelId)) == null)
            {
                throw new ArgumentException("Trying to attach an instance with an unrecognized model.", nameof(instance));
            }
            
            
            InstancesStore.Store(instance);
        }

        public HandleResult HandleEvent(
            Guid processInstanceId,
            Guid tokenId,
            object eventData
        )
        {
            var instance = InstancesStore.GetById(processInstanceId);
            // TODO: Implement a consistent result (and test it)
            if (instance == null)
            {
                return null;
            }

            var model = ModelsStore.GetById(Guid.Parse(instance.ProcessModelId));
            // TODO: Implement a consistent result (and test it) 
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
            object completionData
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

            var tokens = instance.HandleActivityCompletion(context, completionData);

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
            {
                return ModelsStore;
            }

            if (serviceType == typeof(IProcessInstanceProvider))
            {
                return InstancesStore;
            }

            if (serviceType == typeof(IProcessModelsStore))
            {
                return ModelsStore;
            }

            if (serviceType == typeof(IProcessInstancesStore))
            {
                return InstancesStore;
            }

            return _serviceProvider?.GetService(serviceType);
        }
    }
}