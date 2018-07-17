using System;
using TheFlow.Notifications.Application.Infrastructure.Mediatr.Commands;

namespace TheFlow.Notifications.Application.CommandSide.MarkAsProcessed
{
    public class MarkAsProcessedCommand : ICommand<string>
    {
        public Guid ProcessId { get; }
        
        public MarkAsProcessedCommand(Guid processId)
        {
            ProcessId = processId;
        }
    }
}
