using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace TheFlow.Elements.Gateways
{
    public class ParallelGateway : Activity
    {
        public override void Run(ExecutionContext context)
        {
            var incomingConnections = context.Model.GetIncomingConnections(context.Token.ExecutionPoint);

            if (incomingConnections.Count() == 1)
            {
                context.Instance
                    .HandleActivityCompletation(context.Token.Id, context.Model, null);
            }
            else
            {
                var parent = context.Instance.Token.FindById(context.Token.ParentId);
            
                context.Token.Release();

                if (!parent.GetActiveDescendants().Any())
                {
                    parent.ExecutionPoint = context.Token.ExecutionPoint;
                    context.Instance
                        .HandleActivityCompletation(context.Token.ParentId, context.Model, null);
                }
            }
        }
    }
}