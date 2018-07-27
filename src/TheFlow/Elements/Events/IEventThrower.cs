using System;
using TheFlow.CoreConcepts;

namespace TheFlow.Elements.Events
{
    public interface IEventThrower : IElement
    {
        void Throw(ExecutionContext context);
    }
}