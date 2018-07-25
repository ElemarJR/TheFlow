using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheFlow.Elements;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;
using TheFlow.Infrastructure;

namespace TheFlow.CoreConcepts
{
    public class ProcessInstance : IProcessInstanceProvider, IServiceProvider
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
            IServiceProvider serviceProvider,
            object eventData
        )
        {
            var modelProvider = serviceProvider.GetService<IProcessModelProvider>();
            var model = modelProvider.GetProcessModel(Guid.Parse(ProcessModelId));
            return HandleEvent(model, eventData);
        }
        
        // TODO: Throw an exception when ProcessModel does not matches
        public IEnumerable<Token> HandleEvent(
            ProcessModel model,
            object eventData
        ) => HandleEvent(Token.Id, model, eventData);

        public IEnumerable<Token> HandleEvent(
            Guid tokenId, 
            ProcessModel model, 
            object eventData
            )
        {
            var token = Token.FindById(tokenId);

            if (token == null || !token.IsActive)
            {
                return Enumerable.Empty<Token>();
            }

            INamedProcessElement<IEventCatcher> @event;

            
            // TODO: What if it is already running?
            if (!IsRunning)
            {
                

                @event = model.GetStartEventCatchers()
                    .FirstOrDefault(e =>
                    {
                        var ctx = new ExecutionContext(
                            this, model, this, token, e.Element
                        );
                        return e.Element.CanHandle(ctx, eventData);
                    });
                token.ExecutionPoint = @event?.Name;
            }
            else
            {
                @event = model.GetElementByName(token.ExecutionPoint)
                    as INamedProcessElement<IEventCatcher>;
            }
            
            var context = new ExecutionContext(
                this, model, this, token, @event?.Element
            );            

            if (@event == null || !@event.Element.CanHandle(context, eventData))
            {
                return Enumerable.Empty<Token>();
            }

            // TODO: Handle Exceptions
            @event.Element.Handle(context, eventData);
            
            _history.Add(new HistoryItem(
                DateTime.UtcNow, token.Id, token.ExecutionPoint, eventData, "eventCatched"
                ));

            var connections = model
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

            return MoveOn(model, new[] {token});
        }

        private IEnumerable<Token> MoveOn(
            ProcessModel model,
            IEnumerable<Token> tokens
            )
        {
            var result = new List<Token>();
            foreach (var token in tokens)
            {
                while (true)
                {
                    // TODO: Ensure model is valid (all connections are valid)
                    var e = model.GetElementByName(token.ExecutionPoint);
                    var element = e.Element;

                    if (element is Activity a)
                    {
                        // TODO: Improve this
                        var sp = new CumulativeServiceProvider(
                            this,
                            new ServiceCollection()
                                .AddSingleton(model)
                                .BuildServiceProvider()
                        );

                        _history.Add(new HistoryItem(
                            DateTime.UtcNow, token.Id, token.ExecutionPoint, null, "activityStarted"
                        ));
                        
                        var context = new ExecutionContext(
                            sp, model, this, token, a
                            );

                        a.Run(context);
                        break;
                    }
                    else if (element is IEventThrower et)
                    {
                        _history.Add(new HistoryItem(
                            DateTime.UtcNow, 
                            token.Id, token.ExecutionPoint, null, "eventThrown"
                        ));
                        
                        var context = new ExecutionContext(
                            this, model, this, token, et
                        );
                        
                        et.Throw(context);
                        if (model.IsEndEventThrower(token.ExecutionPoint))
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

                    var connections = model.GetOutcomingConnections(
                        token.ExecutionPoint
                    ).ToArray();

                    if (connections.Count() != 1)
                    {
                        throw new NotImplementedException();
                    }

                    token.ExecutionPoint = connections.FirstOrDefault()?.Element.To;
                }

                result.Add(token);
            };
            return result;
        }

        public IEnumerable<Token> HandleActivityCompletation(Guid tokenId, ProcessModel model, object completationData)
        {
            if (!IsRunning)
            {
                return Enumerable.Empty<Token>();
            }

            var token = Token.FindById(tokenId);

            if (token == null || !token.IsActive)
            {
                return Enumerable.Empty<Token>();
            }

            if (!(model.GetElementByName(token.ExecutionPoint) 
                is INamedProcessElement<Activity> activity))
            {
                return Enumerable.Empty<Token>();
            }

            // TODO: Handle Exceptions
            _history.Add(new HistoryItem(
                DateTime.UtcNow, token.Id, token.ExecutionPoint, completationData, "activityCompleted"
            ));

            var connections = model
                .GetOutcomingConnections(activity.Name)
                .ToArray();

            // TODO: Provide a better solution for a bad model structure
            if (!connections.Any())
                throw new NotSupportedException();

            if (connections.Count() > 1)
            {
                return MoveOn(model, connections
                    .Select(connection =>
                    {
                        var child = token.AllocateChild();
                        child.ExecutionPoint = connection.Element.To;
                        
                        return child;
                    })
                    .ToArray());
            }

            token.ExecutionPoint = connections.First().Element.To;

            return MoveOn(model, new[] { token });
        }


        public ProcessInstance GetProcessInstance(Guid id) => 
            id == Guid.Parse(Id) 
                ? this 
                : null;

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ProcessInstance))
                return this;
            
            if (serviceType == typeof(IProcessInstanceProvider))
                return this;
            
            return null;
        }
    }
}