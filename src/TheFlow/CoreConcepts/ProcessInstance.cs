using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TheFlow.Elements.Activities;
using TheFlow.Elements.Events;

namespace TheFlow.CoreConcepts
{
    public partial class ProcessInstance : IProcessInstanceProvider
    {
        private readonly IDictionary<DataInputOutputKey, object> _elementsState;
        public string ProcessModelId { get; }
        public string Id { get; }
        public Token Token { get;  }
        public bool IsRunning => Token.ExecutionPoint != null;
        public bool IsDone { get; private set; }

        private readonly List<HistoryItem> _history;
        public IEnumerable<HistoryItem> History => _history;


        public void SetDataInputValue(
            string elementName, string inputName, object value
        )
        {
            var key = new DataInputOutputKey(elementName, inputName);
            _elementsState[key] = value;
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
            _history.Add(HistoryItem.Create(context.Token, HistoryItemActions.ActitvityStarted));

            logger?.LogInformation($"Activity {context.Token.ExecutionPoint} execution will start now.");
            activity.Run(context.WithRunningElement(activity));
        }

        private void ThrowEvent(ExecutionContext context, 
            ILogger logger, 
            IEventThrower eventThrower)
        {
            _history.Add(HistoryItem.Create(context.Token, HistoryItemActions.EventThrown));

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
                if (connections.Length != 1)
                {
                    throw new NotSupportedException();
                }

                context.Token.ExecutionPoint = connections.FirstOrDefault()?.Element.To;
                ContinueExecutionFromTheContextPoint(context);
            }
        }
    }
}