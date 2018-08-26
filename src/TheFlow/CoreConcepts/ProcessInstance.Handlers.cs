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

            if (!(context.Model.GetElementByName(context.Token.ExecutionPoint) is INamedProcessElement<Activity>))
            {
                return Enumerable.Empty<Token>();
            }

            // TODO: Handle Exceptions
            _history.Add(HistoryItem.Create(context.Token, completionData, HistoryItemActions.ActivityCompleted));

            return ContinueExecutionFromTheContextPointConnections(context);
        }

        public IEnumerable<Token> HandleActivityFailure(ExecutionContext context, object failureData)
        {
            context.Token.ExecutionPoint = "__compensation_start__";
            ContinueExecutionFromTheContextPoint(context);
            return context.Token.GetActionableTokens();
        }
    }
}
