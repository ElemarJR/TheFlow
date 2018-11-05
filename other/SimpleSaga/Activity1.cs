using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace SimpleSaga
{
    public class Activity1 : Activity
    {
        public override void Run(ExecutionContext context)
        {
            Console.WriteLine("Running activity 1. Is it working?");
            var response = Console.ReadKey();

            if (!(response.KeyChar == 'Y' || response.KeyChar == 'y'))
            {
                context
                    .Instance
                    .HandleActivityFailure(context, "failed to run activity 1");
            }
            else
            {
                context
                    .Instance
                    .HandleActivityCompletion(context, null);
            }
        }
    }
}