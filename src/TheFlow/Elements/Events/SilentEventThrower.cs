using System;

namespace TheFlow.Elements.Events
{
    public class SilentEventThrower : IEventThrower
    {
        private SilentEventThrower()
        {
        }
        
        public static readonly SilentEventThrower Instance = 
            new SilentEventThrower();
        
        public void Throw(IServiceProvider sp)
        {}
    }
}