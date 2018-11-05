using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Events;

namespace SimpleSaga
{
    public class EndEventThrower : IEventThrower
    {
        public void Throw(ExecutionContext context)
        {
            Console.WriteLine("Process was completed with no fails.");
        }
    }
}
