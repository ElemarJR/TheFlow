using System.Collections.Generic;
using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Connections;
using TheFlow.Elements.ConnectionsSelectors;

namespace TheFlow.Elements.Gateways
{   public class ExclusiveGateway : Activity, IConnectionsSelector
    {
        public override void Run(ExecutionContext context)
        {
            context.Instance
                .HandleActivityCompletion(
                    context.WithRunningElement(null),
                    null);
        }

        public IEnumerable<IProcessElement<IConnectionElement>> GetRunnableConnections(ExecutionContext context)
            => DefaultConnectionsSelector.Instance
                .GetRunnableConnections(context)
                .Take(1);
    }
}