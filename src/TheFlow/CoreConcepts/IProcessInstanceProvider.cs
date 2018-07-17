using System;
using TheFlow.CoreConcepts;

namespace TheFlow
{
    public interface IProcessInstanceProvider
    {
        ProcessInstance GetProcessInstance(Guid id);
    }
}