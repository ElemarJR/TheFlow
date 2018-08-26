using System;
using System.Linq;

namespace TheFlow.CoreConcepts
{
    partial class ProcessInstance
    {
        public object GetDataInputValue(
            string elementName, string inputName
        )
        {
            var key = new DataInputOutputKey(elementName, inputName);
            return _elementsState.TryGetValue(key, out var result) 
                ? result 
                : null;
        }

        public ProcessInstance GetProcessInstance(Guid id) => 
            id == Guid.Parse(Id) 
                ? this 
                : null;

        public bool WasActivityCompleted(string activityId)
        {
            var result = _history.Any(item =>
                item.ExecutionPoint == activityId &&
                item.Action == HistoryItemActions.ActivityCompleted
            );
            return result;
        }
    }
}
