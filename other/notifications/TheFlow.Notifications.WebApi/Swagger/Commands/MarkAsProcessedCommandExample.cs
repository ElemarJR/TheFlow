using System;
using Swashbuckle.AspNetCore.Examples;
using TheFlow.Notifications.Application.CommandSide.MarkAsProcessed;

namespace TheFlow.Notifications.WebApi.Swagger.Commands
{
    public class MarkAsProcessedCommandExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new MarkAsProcessedCommand(
                    Guid.NewGuid()
                    );
        }
    }
}
