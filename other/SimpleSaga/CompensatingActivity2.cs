using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace SimpleSaga
{
    public class CompensatingActivity2 : Activity
    {
        public override void Run(ExecutionContext context)
        {
            Console.WriteLine("Running compensating activity 2");
            context
                .Instance
                .HandleActivityCompletion(context, null);
        }
    }
}