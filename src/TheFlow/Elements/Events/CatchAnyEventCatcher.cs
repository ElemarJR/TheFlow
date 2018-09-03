using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Data;

namespace TheFlow.Elements.Events
{
    public class CatchAnyEventCatcher 
        : IEventCatcher, IDataProducer
    {
        private CatchAnyEventCatcher()
        {}
        
        public static IEventCatcher Create() 
            => new CatchAnyEventCatcher();

        public bool CanHandle(ExecutionContext context, object @event) => true;

        public void Handle(ExecutionContext context, object @event)
        {
            context.Instance.SetDataObjectValue(context.Token.ExecutionPoint, @event);
            _dataOutput?.Update(context, context.Token.ExecutionPoint, @event);
        }
        
        private DataOutput _dataOutput = new DataOutput("default");

        public void SetEventDataOutput(DataOutput dataOutput)
        {
            _dataOutput = dataOutput;
        }

        public DataOutput GetDataOutputByName(string name) => 
            _dataOutput?.Name == name ? _dataOutput : null;
    }
}