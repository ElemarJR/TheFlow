using TheFlow.Elements.Data;

namespace TheFlow.Elements.Events
{
    public class TypedEventCatcher<TEvent> : IEventCatcher 
        where TEvent : class
    {
        bool IEventCatcher.CanHandle(object @event) => 
            CanHandleImpl(@event as TEvent);

        void IEventCatcher.Handle(object @event)
        {
            HandleImpl(@event as TEvent);
        }

        private DataOutput _dataOutput;
        void IEventCatcher.SetEventDataOutput(DataOutput dataOutput)
        {
            _dataOutput = dataOutput;
        }
        
        public bool CanHandle(TEvent @event) =>
            CanHandleImpl(@event);

        public void Handle(TEvent @event) =>
            HandleImpl(@event);

        protected virtual bool CanHandleImpl(TEvent @event)
        {
            return @event != null;
        }

        protected virtual void HandleImpl(TEvent @event)
        {
            return;
        }
        
        public static readonly TypedEventCatcher<TEvent> Instance =
            new TypedEventCatcher<TEvent>();
    }
}