using System;
using System.Collections.Generic;
using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Validations;

namespace TheFlow.Infrastructure.Stores
{
    public class InMemoryProcessModelsStore : IProcessModelsStore
    {
        private readonly Dictionary<string, ProcessModel> _models = 
            new Dictionary<string, ProcessModel>();

        public InMemoryProcessModelsStore() {}

        public InMemoryProcessModelsStore(params ProcessModel[] models)
        {
            foreach (var model in models)
            {
                Store(model);
            }
        }
        
        public ProcessModel GetById(Guid id)
        {
            return _models.ContainsKey(id.ToString()) ? _models[id.ToString()] : null;
        }

        public void Store(ProcessModel model)
        {
            // TODO: How to ensure that all models are validated
            var validationResults = new EnsureThereAreOutcomingConnectionsWhenNecessaryValidationRule()
                .Validate(model).ToArray();

            if (validationResults.Length > 0)
            {
                throw new InvalidOperationException("Trying to add an invalid model.\n\n" +
                                                    string.Join("\n", validationResults.Select(v => v.Message))
                                                    );
            }

            if (_models.ContainsKey(model.Id))
            {
                _models[model.Id] = model;
            }
            else
            {
                _models.Add(model.Id, model);
            }
        }

        public IEnumerable<ProcessModel> GetAll() =>
            _models.Values;

        public ProcessModel GetProcessModel(Guid id) 
            => GetById(id);
    }
}