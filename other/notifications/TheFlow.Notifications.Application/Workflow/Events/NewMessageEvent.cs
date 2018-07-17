namespace TheFlow.Notifications.Application.Workflow.Events
{
    public class NewMessageEvent
    {
        public string From { get; }
        public string To { get; }
        public string Message { get; }
        public string ActionUrl { get; }

        public NewMessageEvent(
            string from,
            string to,
            string message,
            string actionUrl
        )
        {
            From = @from;
            To = to;
            Message = message;
            ActionUrl = actionUrl;
        }
    }
}
