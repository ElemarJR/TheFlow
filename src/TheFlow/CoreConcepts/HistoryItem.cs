using System;

namespace TheFlow.CoreConcepts
{
    public class HistoryItem
    {
        public HistoryItem(DateTime @when, Guid tokenId, string executionPoint, object payload, string action)
        {
            When = when;
            TokenId = tokenId;
            ExecutionPoint = executionPoint;
            Payload = payload;
            Action = action;
        }

        public DateTime When { get; }
        public Guid TokenId { get; }
        public string ExecutionPoint { get; }
        public object Payload { get;  }
        public string Action { get; }
    }
}