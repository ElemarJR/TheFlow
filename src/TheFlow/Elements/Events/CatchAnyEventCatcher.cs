using System;

namespace TheFlow.Elements.Events
{
    public class CatchAnyEventCatcher : IEventCatcher
    {
        private CatchAnyEventCatcher()
        {}
        
        public static readonly IEventCatcher Instance =
            new CatchAnyEventCatcher();
        
        public bool CanHandle(object @event) => true;

        public void Handle(object @event) {}
        
        public static CatchAnyEventCatcherBuilder Builder() 
            => new CatchAnyEventCatcherBuilder();
    }

    public class CatchAnyEventCatcherBuilder
    {
        public CatchAnyEventCatcherBuilder AddDataOutput(string eventdata)
        {
            throw new System.NotImplementedException();
        }

        public CatchAnyEventCatcher Build()
        {
            throw new NotImplementedException();
        }

        public static implicit operator CatchAnyEventCatcher(CatchAnyEventCatcherBuilder builder)
            => builder.Build();
    }
}