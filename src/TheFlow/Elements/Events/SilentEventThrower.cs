using TheFlow.CoreConcepts;

namespace TheFlow.Elements.Events
{
    public class SilentEventThrower : IEventThrower
    {
        private SilentEventThrower()
        {
        }
        
        public static readonly SilentEventThrower Instance = 
            new SilentEventThrower();

        public void Throw(ExecutionContext context)
        {
            // This implementation will not throw any event. It is silent!
        }
    }
}