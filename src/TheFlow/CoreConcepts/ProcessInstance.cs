using System;
using System.Collections.Generic;
using System.Linq;
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
            string id,
            Token token = null,
            IEnumerable<HistoryItem> history = null,
            IDictionary<DataInputOutputKey, object> elementsState = null
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
            
            var token = context.Token;

            if (token == null || !token.IsActive)
            {
                return Enumerable.Empty<Token>();
            }

            INamedProcessElement<IEventCatcher> @event;

            
            if (!IsRunning)
            {
                @event = context.Model.GetStartEventCatchers()
                    .FirstOrDefault(e => e.Element.CanHandle(context.WithRunningElement(e.Element), eventData));
                token.ExecutionPoint = @event?.Name;
            }
            else
            {
                @event = context.Model.GetElementByName(token.ExecutionPoint)
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
                DateTime.UtcNow, token.Id, token.ExecutionPoint, eventData, "eventCatched"
                ));

            var connections = ctx.Model
                .GetOutcomingConnections(@event.Name)
                .ToArray();

            // TODO: Provide a better solution for a bad model structure
            if (!connections.Any())
                throw new NotSupportedException();

            if (connections.Count() > 1)
            {
                return connections
                    .Select(connection =>
                    {
                        var child = token.AllocateChild();
                        child.ExecutionPoint = connection.Element.To;
                        return child;
                    })
                    .ToList();
            }

            token.ExecutionPoint = connections.First().Element.To;

            MoveOn(ctx, new[] {token});

            return token.GetActionableTokens();
        }

        private void MoveOn(
            ExecutionContext context,
            IEnumerable<Token> tokens
            )
        {
            var logger = context.ServiceProvider?
                .GetService<ILogger<ProcessInstance>>();
    
            foreach (var token in tokens)
            {
                var scope = logger?.BeginScope($"{token.Id}, {token.ExecutionPoint}");
                //logger?.LogInformation($"Working on token {token.Id}...");

                while (true)
                {
                    // TODO: Ensure model is valid (all connections are valid)
                    var e = context.Model.GetElementByName(token.ExecutionPoint);
                    var element = e.Element;
                    logger?.LogInformation($"Performing {e.Name} ...");


                    if (element is Activity a)
                    {
                        
                        _history.Add(new HistoryItem(
                            DateTime.UtcNow, token.Id, token.ExecutionPoint, null, "activityStarted"
                        ));

                        var ctx = context.WithToken(token).WithRunningElement(a);
                        a.Run(ctx);
                        
                        break;
                    }

                    if (element is IEventThrower et)
                    {
                        _history.Add(new HistoryItem(
                            DateTime.UtcNow, 
                            token.Id, token.ExecutionPoint, null, "eventThrown"
                        ));


                        var ctx = context.WithToken(token).WithRunningElement(et);
                        et.Throw(ctx);
                        
                        if (context.Model.IsEndEventThrower(token.ExecutionPoint))
                        {
                            token.ExecutionPoint = null;
                            token.Release();
                            IsDone = true;
                            break;
                        }
                    }
                    else if (element is IEventCatcher)
                    {
                        break;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    var connections = context.Model.GetOutcomingConnections(
                        token.ExecutionPoint
                    ).ToArray();

                    if (connections.Count() != 1)
                    {
                        throw new NotImplementedException();
                    }

                    token.ExecutionPoint = connections.FirstOrDefault()?.Element.To;
                }
                scope?.Dispose();
            }
        }

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

            if (!(context.Model.GetElementByName(context.Token.ExecutionPoint) 
                is INamedProcessElement<Activity> activity))
            {
                return Enumerable.Empty<Token>();
            }

            // TODO: Handle Exceptions
            _history.Add(new HistoryItem(
                DateTime.UtcNow, context.Token.Id, context.Token.ExecutionPoint, completionData, "activityCompleted"
            ));

            var connections = context.Model
                .GetOutcomingConnections(activity.Name)
                .ToArray();

            // TODO: Provide a better solution for a bad model structure
            if (!connections.Any())
                throw new NotSupportedException();

            
            if (connections.Count() > 1)
            {
                
                MoveOn(context, connections
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
                MoveOn(context, new[] { context.Token });
                
            }
            return context.Token.GetActionableTokens();
        }

        public ProcessInstance GetProcessInstance(Guid id) => 
            id == Guid.Parse(Id) 
                ? this 
                : null;

    }
}