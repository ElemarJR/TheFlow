using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheFlow.Elements.Activities;
using TheFlow.Elements.ConnectionsSelectors;
using TheFlow.Elements.Events;

namespace TheFlow.CoreConcepts
{
    partial class ProcessInstance
    {
        private void ContinueExecutionFromTheContextPoint(
            ExecutionContext context
        )
        {
            var logger = context.ServiceProvider?
                .GetService<ILogger<ProcessInstance>>();

            // TODO: Ensure model is valid (all connections are valid)
            var e = context.Model.GetElementByName(context.Token.ExecutionPoint);
            var element = e.Element;

            switch (element)
            {
                case IEventCatcher _:
                    break;
                case Activity a:
                    {
                        _history.Add(HistoryItem.Create(context.Token, HistoryItemActions.ActitvityStarted));
                        logger?.LogInformation($"Activity {context.Token.ExecutionPoint} execution will start now.");
                        a.Run(context.WithRunningElement(a));
                        break;
                    }
                case IEventThrower et:
                    {
                        _history.Add(HistoryItem.Create(context.Token, HistoryItemActions.EventThrown));
                        et.Throw(context.WithRunningElement(et));
                        logger?.LogInformation($"Event {context.Token.ExecutionPoint} was thrown.");
                        ContinueExecutionFromTheContextPointConnections(context);
                        break;
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        private void ContinueExecutionForAllTokensInParallel(
            ExecutionContext context,
            IEnumerable<Token> tokens
        )
        {

            var enumerable = tokens as Token[] ?? tokens.ToArray();
            if (enumerable.Length == 1)
            {
                ContinueExecutionFromTheContextPoint(context.WithToken(enumerable[0]));
            }
            else
            {
                Parallel.ForEach(enumerable, token =>
                {
                    ContinueExecutionFromTheContextPoint(context.WithToken(token));
                });
            }
        }

        private IEnumerable<Token> ContinueExecutionFromTheContextPointConnections(
            ExecutionContext context
        )
        {
            if (context.Model.IsEndEventThrower(context.Token.ExecutionPoint))
            {
                context.Token.ExecutionPoint = null;
                context.Token.Release();
                IsDone = true;
            }
            else
            {

                var selector =
                    context.Model.GetElementByName(context.Token.ExecutionPoint).Element as IConnectionsSelector
                    ?? DefaultConnectionsSelector.Instance;

                var connections = selector
                    .GetRunnableConnections(context)
                    .ToArray();

                // TODO: Provide a better solution for a bad model structure
                if (!connections.Any())
                    throw new NotSupportedException();

                if (connections.Length > 1)
                {
                    ContinueExecutionForAllTokensInParallel(context, connections
                        .Select(connection =>
                        {
                            var child = context.Token.AllocateChild();
                            child.ExecutionPoint = connection.Element.To;
                            return child;
                        }));
                }
                else
                {
                    context.Token.ExecutionPoint = connections.First().Element.To;
                    ContinueExecutionFromTheContextPoint(context);
                }
            }

            return context.Token.GetActionableTokens();
        }
    }
}
