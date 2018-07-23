using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Data;

namespace TheFlow.Elements.Events
{
    public interface IEventCatcher : IElement
    {
        bool CanHandle(ExecutionContext context, object @event);
        void Handle(ExecutionContext context, object @event);

        void SetEventDataOutput(
            DataOutput dataOutput
        );
    }
}