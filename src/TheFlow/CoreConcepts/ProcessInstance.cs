using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheFlow.Elements;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;

namespace TheFlow.CoreConcepts
{
    public class ProcessInstance : IProcessInstanceProvider
    {
        private readonly IDictionary<DataInputOutputKey, object> _elementsState;
        public string ProcessModelId { get; }
        public string Id { get; }
        public Token Token { get;  }
        public bool IsRunning => Token.ExecutionPoint != null;
        public bool IsDone { get; private set; }

        private readonly List<HistoryItem> _history;
        public IEnumerable<HistoryItem> History => _history;

        // TODO: Test serialization with RavenDB
        public ProcessInstance(
            string processModelId,
            string id
        ) : this(processModelId, id, null)
        {
        }

        public ProcessInstance(
            string processModelId,
            string id,
            Token token
        ) : this(processModelId, id, token, null)
        {
        }

        public ProcessInstance(
            string processModelId,
            string id,
            Token token,
            IEnumerable<HistoryItem> history
        ) : this(processModelId, id, token, history, null)
        {
        }

        public ProcessInstance(
            string processModelId,
            string id,
            Token token,
            IEnumerable<HistoryItem> history,
            IDictionary<DataInputOutputKey, object> elementsState
            )
        {
            
            ProcessModelId = processModelId;
            Id = id;
            Token = token ?? Token.Create();
            _history = history?.ToList() ?? new List<HistoryItem>();
            _elementsState = elementsState ?? new Dictionary<DataInputOutputKey, object>();
        }

        public object GetDataInputValue(
            string elementName, string inputName
        )
        {
            var key = new DataInputOutputKey(elementName, inputName);
            if (_elementsState.TryGetValue(key, out var result))
            {
                return result;
            }

            return null;
        }

        public void SetDataInputValue(
            string elementName, string inputName, object value
        )
        {
            var key = new DataInputOutputKey(elementName, inputName);
            _elementsState[key] = value;
        }
        
        public static ProcessInstance Create(string processModelId)
            => Create(Guid.Parse(processModelId));
        
        public static ProcessInstance Create(Guid processModelId) 
            => new ProcessInstance(processModelId.ToString(), Guid.NewGuid().ToString());
        
        public static ProcessInstance Create(ProcessModel model)
            => Create(model.Id);

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
                DateTime.UtcNow, context.Token.Id, context.Token.ExecutionPoint, eventData, "eventCatched"
                ));

            var connections = ctx.Model
                .GetOutcomingConnections(@event.Name)
                .ToArray();

            // TODO: Provide a better solution for a bad model structure
            if (!connections.Any())
            {
                throw new NotSupportedException();
            }

            if (connections.Count() > 1)
            {
                return connections
                    .Select(connection =>
                    {
                        var child = context.Token.AllocateChild();
                        child.ExecutionPoint = connection.Element.To;
                        return child;
                    })
                    .ToList();
            }
            else
            {
                context.Token.ExecutionPoint = connections.First().Element.To;
                MoveOn(ctx, new[] {context.Token});
                return context.Token.GetActionableTokens();
                
            }
        }

        private void MoveOn(
            ExecutionContext context,
            IEnumerable<Token> tokens
        )
        {
            var logger = context.ServiceProvider?
                .GetService<ILogger<ProcessInstance>>();

            var enumerable = tokens as Token[] ?? tokens.ToArray();
            if (enumerable.Count() == 1)
            {
                MoveOn(context.WithToken(enumerable.First()), logger);
            }
            else
            {
                Parallel.ForEach(enumerable, token =>
                {
                    MoveOn(context.WithToken(token), logger);
                });
            }
        }

        private void MoveOn(
            ExecutionContext context, 
            ILogger logger
            )
        {

            // TODO: Ensure model is valid (all connections are valid)
            var e = context.Model.GetElementByName(context.Token.ExecutionPoint);
            var element = e.Element;
            //logger?.LogInformation($"Performing {e.Name} ...");
            
            switch (element)
            {
                case IEventCatcher _:
                    break;
                case Activity a:
                {
                    RunActivity(context, logger, a);
                    break;
                }
                case IEventThrower et:
                {
                    ThrowEvent(context, logger, et);
                    break;
                }
                default:
                    throw new NotSupportedException();
            }
        }

        private void RunActivity(ExecutionContext context, 
            ILogger logger,
            Activity activity)
        {
            _history.Add(HistoryItem.Create(context.Token, "activityStarted"));

            logger?.LogInformation($"Activity {context.Token.ExecutionPoint} execution will start now.");
            activity.Run(context.WithRunningElement(activity));
        }

        private void ThrowEvent(ExecutionContext context, 
            ILogger logger, 
            IEventThrower eventThrower)
        {
            _history.Add(HistoryItem.Create(context.Token, "eventThrow"));

            eventThrower.Throw(context.WithRunningElement(eventThrower));
            logger?.LogInformation($"Event {context.Token.ExecutionPoint} was thrown.");

            if (context.Model.IsEndEventThrower(context.Token.ExecutionPoint))
            {
                context.Token.ExecutionPoint = null;
                context.Token.Release();
                IsDone = true;
            }
            else
            {
                var connections = context.Model.GetOutcomingConnections(
                    context.Token.ExecutionPoint
                ).ToArray();

                // TODO: Move this to the model validation
                if (connections.Count() != 1)
                {
                    throw new NotSupportedException();
                }

                context.Token.ExecutionPoint = connections.FirstOrDefault()?.Element.To;
                MoveOn(context, logger);
            }
        }


        public IEnumerable<Token> HandleActivityCompletion(ExecutionContext context, object completionData)
        {
            
            var logger = context.ServiceProvider?
                .GetService<ILogger<ProcessInstance>>();

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
            _history.Add(HistoryItem.Create(context.Token, completionData, "activityCompleted"));

            var connections = context.Model
                .GetOutcomingConnections(activity.Name)
                .ToArray();

            // TODO: Provide a better solution for a bad model structure
            if (!connections.Any())
                throw new NotSupportedException();
            
            if (connections.Length > 1)
            {
                
                MoveOn(context, connections
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
                MoveOn(context, logger);
                
            }
            return context.Token.GetActionableTokens();
        }

        public ProcessInstance GetProcessInstance(Guid id) => 
            id == Guid.Parse(Id) 
                ? this 
                : null;

    }
}