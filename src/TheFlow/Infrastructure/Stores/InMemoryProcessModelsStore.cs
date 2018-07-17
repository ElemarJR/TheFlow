using System;
using System.Collections.Generic;
using TheFlow.CoreConcepts;

namespace TheFlow.Infrastructure.Stores
{
    public class InMemoryProcessModelsStore : IProcessModelsStore
    {
        private readonly Dictionary<string, ProcessModel> _models = 
            new Dictionary<string, ProcessModel>();
        
        public ProcessModel GetById(Guid id)
        {
            return _models.ContainsKey(id.ToString()) ? _models[id.ToString()] : null;
        }

        public void Store(ProcessModel model)
        {
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