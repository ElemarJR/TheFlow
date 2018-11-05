using System;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace SimpleSaga
{
    public class Activity3 : Activity
    {
        public override void Run(ExecutionContext context)
        {
            Console.WriteLine("Running activity 3. Is it working?");
            var response = Console.ReadKey();

            if (!(response.KeyChar == 'Y' || response.KeyChar == 'y'))
            {
                context
                    .Instance
                    .HandleActivityFailure(context, null);
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