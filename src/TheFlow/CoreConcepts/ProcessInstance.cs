using System.Collections.Generic;

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

        public IDictionary<string, object> EmbeddedDataStoresValues { get; }
        public IDictionary<string, object> DataObjectsValues { get; }


        public void SetDataInputValue(
            string elementName, string inputName, object value
        )
        {
            var key = new DataInputOutputKey(elementName, inputName);
            _elementsState[key] = value;
        }

        public void SetDataObjectValue(string dataObjectName, object value)
        {
            DataObjectsValues[dataObjectName] = value;
        }

        public object GetDataObjectValue(string dataObjectName)
        {
            return DataObjectsValues[dataObjectName];
        }
    }
}