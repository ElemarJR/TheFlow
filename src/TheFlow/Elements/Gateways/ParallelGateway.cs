using System.Linq;
using Microsoft.Extensions.Logging;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;

namespace TheFlow.Elements.Gateways
{
    public class ParallelGateway : Activity
    {
        public override void Run(ExecutionContext context)
        {
            var logger = context.ServiceProvider?
                .GetService<ILogger<ParallelGateway>>();

            var incomingConnections = context.Model
                .GetIncomingConnections(context.Token.ExecutionPoint)
                .ToArray();
            

            if (incomingConnections.Length == 1)
            {
                context.Instance
                    .HandleActivityCompletion(context.WithRunningElement(null), null);
            }
            else
            {
                var parent = context.Instance.Token.FindById(context.Token.ParentId);
            
                context.Token.Release();
                var descendants = parent.GetActiveDescendants();
                
                
                if ( parent.Children.Count() == incomingConnections.Length && !parent.GetActiveDescendants().Any())
                {
                    logger?.LogInformation($"({context.Token.ExecutionPoint}) All tokens are done. Moving on...");

                    parent.ExecutionPoint = context.Token.ExecutionPoint;
                    context.Instance
                        .HandleActivityCompletion(
                            context
                                .WithRunningElement(null)
                                .WithToken(parent), 
                            null);
                }
                else
                {
                    logger?.LogInformation($"({context.Token.ExecutionPoint}) Still waiting for {incomingConnections.Length - parent.Children.Count()} descendants...");
                }
            }
        }
    }
}