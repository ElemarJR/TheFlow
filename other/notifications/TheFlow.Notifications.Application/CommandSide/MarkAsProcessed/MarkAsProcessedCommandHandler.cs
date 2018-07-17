using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TheFlow.CoreConcepts;
using TheFlow.Notifications.Application.Workflow.Events;

namespace TheFlow.Notifications.Application.CommandSide.MarkAsProcessed
{
    public class MarkAsProcessedCommandHandler
        : IRequestHandler<MarkAsProcessedCommand, string>
    {
        private readonly ProcessManager _manager;

        public MarkAsProcessedCommandHandler(ProcessManager manager)
        {
            _manager = manager;
        }

        public Task<string> Handle(MarkAsProcessedCommand request, CancellationToken cancellationToken)
        {
            HandleResult result = null;

            var instance = _manager.InstancesStore.GetById(request.ProcessId);

            if (instance != null)
            { 
                result = _manager.HandleEvent(
                    request.ProcessId,
                    instance.Token.Id,
                    new MessageProcessedEvent()
                );
            }

            return Task.FromResult(
                result?.ProcessInstanceId.ToString() 
                );
        }
    }
}
