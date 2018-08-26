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
    }
}