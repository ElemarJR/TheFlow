using System;
using TheFlow.Elements.Data;

namespace TheFlow.Elements.Events
{
    public class TypedEventCatcher<TEvent> 
        : IEventCatcher, IDataProducer
        where TEvent : class
    {
        bool IEventCatcher.CanHandle(IServiceProvider sp, object @event) => 
            CanHandleImpl(sp, @event as TEvent);

        void IEventCatcher.Handle(IServiceProvider sp, object @event)
        {
            HandleImpl(sp, @event as TEvent);
        }

        private DataOutput _dataOutput;
        void IEventCatcher.SetEventDataOutput(DataOutput dataOutput)
        {
            _dataOutput = dataOutput;
        }
        
        public bool CanHandle(IServiceProvider sp,TEvent @event) =>
            CanHandleImpl(sp, @event);

        public void Handle(IServiceProvider sp, TEvent @event) =>
            HandleImpl(sp, @event);

        protected virtual bool CanHandleImpl(IServiceProvider sp, TEvent @event)
        {
            return @event != null;
        }

        protected virtual void HandleImpl(IServiceProvider sp, TEvent @event)
        {
            return;
        }
        
        public static TypedEventCatcher<TEvent> Create() =>
            new TypedEventCatcher<TEvent>();

        public DataOutput GetDataOutputByName(string name) =>
            _dataOutput?.Name == name ? _dataOutput : null;
    }
}