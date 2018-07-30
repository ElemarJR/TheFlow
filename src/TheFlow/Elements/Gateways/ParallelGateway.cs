using System.Linq;
using Microsoft.Extensions.Logging;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Activities;
using TheFlow.Infrastructure.Parallel;

namespace TheFlow.Elements.Gateways
{
    public class ParallelGateway : Activity
    {
        // TODO: Run in Parallel
        public override void Run(ExecutionContext context)
        {
            var processMonitor = context.ServiceProvider
                .GetService<IProcessMonitor>();
            
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
                var parentToken = context.Instance.Token.FindById(context.Token.ParentId);

                var key = $"{context.Token.ParentId}/{context.Token.ExecutionPoint}";
                bool readyToGo;
                using (processMonitor?.Lock(key))
                {
                    context.Token.Release();
                    var childrenCount = parentToken.Children.Count();
                    var actionableChildrenCount = parentToken.Children
                        .SelectMany(c => c.GetActionableTokens())
                        .Count();
                    
                    readyToGo = actionableChildrenCount == 0 && childrenCount == incomingConnections.Length;
                }

                if (readyToGo)
                {
                    logger?.LogInformation($"({context.Token.ExecutionPoint}) All tokens are done. Moving on...");

                    parentToken.ExecutionPoint = context.Token.ExecutionPoint;
                    
                    context.Instance
                        .HandleActivityCompletion(
                            context
                                .WithRunningElement(null)
                                .WithToken(parentToken), 
                            null);
                }
                else
                {
                    logger?.LogInformation($"({context.Token.ExecutionPoint}) Still waiting for other descendants...");
                }
            }
        }
    }
}