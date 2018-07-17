using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheFlow.Notifications.Application.Workflow.Events;

namespace TheFlow.Notifications.Application.CommandSide.NewMessage
{
    public class NewMessageCommandHandler
        : IRequestHandler<NewMessageCommand, string>
    {
        private readonly ProcessManager _manager;

        public NewMessageCommandHandler(ProcessManager manager)
        {
            _manager = manager;
        }

        public Task<string> Handle(NewMessageCommand request, CancellationToken cancellationToken)
        {
            var @event = new NewMessageEvent(
                request.From,
                request.To,
                request.Message,
                request.ActionUrl
            );

            var result = _manager.HandleEvent(@event).FirstOrDefault();
            
            return Task.FromResult(
                result?.ProcessInstanceId.ToString() 
                );
        }
    }
}
