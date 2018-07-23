using System;

namespace TheFlow.Elements.Events
{
    public interface IEventThrower : IElement
    {
        void Throw(IServiceProvider serviceProvider);
    }
}