using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace TheFlow.Elements.Gateways
{
    // TODO: Ensure there is a single path (first to run)
    public class ExclusiveGateway : Activity
    {
        public override void Run(ExecutionContext context)
        {
            context.Instance
                .HandleActivityCompletion(
                    context
                        .WithRunningElement(null)
                        , 
                    null);
        }
    }
}