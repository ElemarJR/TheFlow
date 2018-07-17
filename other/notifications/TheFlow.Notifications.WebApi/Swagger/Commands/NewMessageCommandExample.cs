using Swashbuckle.AspNetCore.Examples;
using TheFlow.Notifications.Application.CommandSide.NewMessage;

namespace TheFlow.Notifications.WebApi.Swagger.Commands
{
    public class NewMessageCommandExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewMessageCommand(
                    from: "fromUserId",
                    to: "toUserId",
                    message: "Hello, World.",
                    actionUrl: "http://google.com"
                    );
        }
    }
}
