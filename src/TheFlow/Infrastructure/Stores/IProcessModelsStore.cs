using System;
using System.Collections.Generic;
using TheFlow.CoreConcepts;

namespace TheFlow.Infrastructure.Stores
{
    public interface IProcessModelsStore
        : IProcessModelProvider
    {
        ProcessModel GetById(Guid id);
        IEnumerable<ProcessModel> GetAll();
        void Store(ProcessModel model);
    }
}