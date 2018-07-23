using System;
using TheFlow.CoreConcepts;

namespace TheFlow.Elements.Activities
{
    public abstract class Activity : IElement
    {
        public abstract void Run(
            ExecutionContext context 
            );
    }
}