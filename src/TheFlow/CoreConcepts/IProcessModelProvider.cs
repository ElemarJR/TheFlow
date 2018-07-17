using System;

namespace TheFlow.CoreConcepts
{
    public interface IProcessModelProvider
    {
        ProcessModel GetProcessModel(Guid id);
    }
}
