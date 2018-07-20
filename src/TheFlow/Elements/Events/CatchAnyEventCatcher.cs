using TheFlow.Elements.Data;

namespace TheFlow.Elements.Events
{
    public class CatchAnyEventCatcher : IEventCatcher
    {
        private CatchAnyEventCatcher()
        {}
        
        public static IEventCatcher Create() 
            => new CatchAnyEventCatcher();

        public bool CanHandle(object @event) => true;

        public void Handle(object @event) {}
        
        private DataOutput _dataOutput;

        public void SetEventDataOutput(DataOutput dataOutput)
        {
            _dataOutput = dataOutput;
        }
    }
}