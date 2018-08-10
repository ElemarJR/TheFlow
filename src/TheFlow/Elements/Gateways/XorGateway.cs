using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace TheFlow.Elements.Gateways
{
    public class XorGateway : Activity
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