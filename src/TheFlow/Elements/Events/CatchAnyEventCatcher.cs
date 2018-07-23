using System;
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

        public bool CanHandle(IServiceProvider sp, object @event) => true;

        public void Handle(IServiceProvider sp, object @event) {}
        
        private DataOutput _dataOutput;

        public void SetEventDataOutput(DataOutput dataOutput)
        {
            _dataOutput = dataOutput;
        }

        public DataOutput GetDataOutputByName(string name) => 
            _dataOutput?.Name == name ? _dataOutput : null;
    }
}