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

        public static HistoryItem Create(Token token, string action)
            => Create(token, null, action);
        
        public static HistoryItem Create(Token token, object payload, string action)
            => new HistoryItem(DateTime.Now, token.Id, token.ExecutionPoint, payload, action);
    }

    public static class HistoryItemActions
    {
        public static readonly string EventThrown = "eventThrown";
        public static readonly string ActitvityStarted = "activityStarted";
        public static readonly string ActivityCompleted = "activityCompleted";
        public static readonly string EventCatched = "eventCatched";
    }
}