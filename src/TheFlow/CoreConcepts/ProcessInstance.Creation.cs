using System;
using System.Collections.Generic;
using System.Linq;

namespace TheFlow.CoreConcepts
{
    partial class ProcessInstance
    {
        // TODO: Test serialization with RavenDB
        public ProcessInstance(
            string processModelId,
            string id) : this(processModelId, id, null)
        {
        }

        public ProcessInstance(
            string processModelId,
            string id,
            Token token) : this(processModelId, id, token, null)
        {
        }

        public ProcessInstance(
            string processModelId,
            string id,
            Token token,
            IEnumerable<HistoryItem> history
            ) : this(processModelId, id, token, history, null, null)
        {
        }

        public ProcessInstance(
            string processModelId,
            string id,
            Token token,
            IEnumerable<HistoryItem> history,
            IDictionary<DataInputOutputKey, object> elementsState
        ) : this(processModelId, id, token, history, elementsState, null)
        {
        }

        public ProcessInstance(
            string processModelId,
            string id,
            Token token,
            IEnumerable<HistoryItem> history,
            IDictionary<DataInputOutputKey, object> elementsState, 
            IDictionary<string, object> embeddedDataStoresValues
        ) : this(processModelId, id, token, history, elementsState, embeddedDataStoresValues, null)
        {
        }

        public ProcessInstance(
            string processModelId,
            string id,
            Token token,
            IEnumerable<HistoryItem> history,
            IDictionary<DataInputOutputKey, object> elementsState, 
            IDictionary<string, object> embeddedDataStoresValues,
            IDictionary<string, object> dataObjectsValues
            )
        {
            
            ProcessModelId = processModelId;
            Id = id;
            Token = token ?? Token.Create();
            _history = history?.ToList() ?? new List<HistoryItem>();
            _elementsState = elementsState ?? new Dictionary<DataInputOutputKey, object>();
            EmbeddedDataStoresValues = embeddedDataStoresValues ?? new Dictionary<string, object>();
            DataObjectsValues = dataObjectsValues ?? new Dictionary<string, object>(); 
        }

        public static ProcessInstance Create(string processModelId)
            => Create(Guid.Parse(processModelId));

        public static ProcessInstance Create(Guid processModelId)
            => new ProcessInstance(processModelId.ToString(), Guid.NewGuid().ToString());

        public static ProcessInstance Create(ProcessModel model)
            => Create(model.Id);


    }
}
