using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace SimpleSaga
{
    public class CompensatoryActivity1 : Activity
    {
        public override void Run(ExecutionContext context)
        {
            Console.WriteLine("Running compensating activity 1");
            context
                .Instance
                .HandleActivityCompletion(context, null);
        }
    }
}