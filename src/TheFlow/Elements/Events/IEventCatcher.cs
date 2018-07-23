using System;
using TheFlow.Elements.Data;

namespace TheFlow.Elements.Events
{
    public interface IEventCatcher : IElement
    {
        bool CanHandle(IServiceProvider serviceProvider, object @event);
        void Handle(IServiceProvider serviceProvider, object @event);

        void SetEventDataOutput(
            DataOutput dataOutput
        );
    }
}