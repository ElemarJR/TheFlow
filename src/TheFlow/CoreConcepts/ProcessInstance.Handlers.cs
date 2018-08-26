using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TheFlow.Elements;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;

namespace TheFlow.CoreConcepts
{
    partial class ProcessInstance
    {
        public IEnumerable<Token> HandleEvent(
            ExecutionContext context,
            object eventData
        )
        {
            var logger = context.ServiceProvider?
                .GetService<ILogger<ProcessInstance>>();

            logger?.LogInformation($"Handling Event");

            if (context.Token == null || !context.Token.IsActive)
            {
                return Enumerable.Empty<Token>();
            }

            INamedProcessElement<IEventCatcher> @event;


            if (!IsRunning)
            {
                @event = context.Model.GetStartEventCatchers()
                    .FirstOrDefault(e => e.Element.CanHandle(context.WithRunningElement(e.Element), eventData));
                context.Token.ExecutionPoint = @event?.Name;
            }
            else
            {
                @event = context.Model.GetElementByName(context.Token.ExecutionPoint)
                    as INamedProcessElement<IEventCatcher>;
            }

            var ctx = context.WithRunningElement(@event?.Element);

            if (@event == null || !@event.Element.CanHandle(ctx, eventData))
            {
                return Enumerable.Empty<Token>();
            }

            // TODO: Handle Exceptions
            @event.Element.Handle(ctx, eventData);

            _history.Add(new HistoryItem(
                DateTime.UtcNow, context.Token.Id, context.Token.ExecutionPoint, eventData, HistoryItemActions.EventCatched
                ));

            //var connections = ctx.Model
            //    .GetOutcomingConnections(@event.Name)
            //    .ToArray();

            //// TODO: Provide a better solution for a bad model structure
            //if (!connections.Any())
            //{
            //    throw new NotSupportedException();
            //}

            //if (connections.Length > 1)
            //{
            //    return connections
            //        .Select(connection =>
            //        {
            //            var child = context.Token.AllocateChild();
            //            child.ExecutionPoint = connection.Element.To;
            //            return child;
            //        })
            //        .ToList();
            //}
            //else
            //{
            //    context.Token.ExecutionPoint = connections.First().Element.To;
            //    ContinueExecutionForAllTokensInParallel(ctx, new[] { context.Token });
            //    return context.Token.GetActionableTokens();

            //}

            return ContinueExecutionFromTheContextPointConnections(context);
        }

        // TODO: Invalidate parallel tokens (?!)
        // TODO: Wait for pending tokens (?!)

        public IEnumerable<Token> HandleActivityCompletion(ExecutionContext context, object completionData)
        {
            if (!IsRunning)
            {
                return Enumerable.Empty<Token>();
            }

            if (context.Token == null || !context.Token.IsActive)
            {
                return Enumerable.Empty<Token>();
            }

            if (!(context.Model.GetElementByName(context.Token.ExecutionPoint) is INamedProcessElement<Activity> activity))
            {
                return Enumerable.Empty<Token>();
            }

            // TODO: Handle Exceptions
            _history.Add(HistoryItem.Create(context.Token, completionData, HistoryItemActions.ActivityCompleted));

            return ContinueExecutionFromTheContextPointConnections(context);
        }

        private IEnumerable<Token> ContinueExecutionFromTheContextPointConnections(
            ExecutionContext context
            )
        {
            var connections = context.Model
                .GetOutcomingConnections(context.Token.ExecutionPoint)
                .ToArray();

            // TODO: Provide a better solution for a bad model structure
            if (!connections.Any())
                throw new NotSupportedException();

            if (connections.Length > 1)
            {

                ContinueExecutionForAllTokensInParallel(context, connections
                    .Where(connection =>
                    {
                        if (!connection.Element.HasFilterValue)
                        {
                            return true;
                        }

                        if (connection.Element.FilterValue == null)
                        {
                            return context.Token.LastDefaultOutput == null;
                        }

                        return connection.Element.FilterValue.Equals(
                            context.Token.LastDefaultOutput
                        );
                    })
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
            return context.Token.GetActionableTokens();
        }

        public IEnumerable<Token> HandleActivityFailure(ExecutionContext context, object failureData)
        {
            var logger = context.ServiceProvider?
                .GetService<ILogger<ProcessInstance>>();

            context.Token.ExecutionPoint = "__compensation_start__";
            ContinueExecutionFromTheContextPoint(context);
            return context.Token.GetActionableTokens();
        }
    }
}
