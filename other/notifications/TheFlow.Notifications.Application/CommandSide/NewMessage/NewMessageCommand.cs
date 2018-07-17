using TheFlow.Notifications.Application.Infrastructure.Mediatr.Commands;

namespace TheFlow.Notifications.Application.CommandSide.NewMessage
{
    public class NewMessageCommand : ICommand<string>
    {
        public string From { get; }
        public string To { get; }
        public string Message { get; }
        public string ActionUrl { get; }

        public NewMessageCommand(
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
